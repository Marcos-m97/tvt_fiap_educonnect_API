import {
  createContext,
  useContext,
  useEffect,
  useState
} from "react";
import { api } from "../services/api";

/* =========================
   TIPOS
========================= */

export type Usuario = {
  id: number;
  nome: string;
  email: string;
  tipo: number; // 0=Admin | 1=Professor | 2=Aluno
};

type Perfil = {
  departamento?: string;
  cargo?: string;
};

type LoginResponse = {
  token: string;
};

type MeResponse = {
  usuario: Usuario;
  perfil: Perfil;
};

type AuthContextType = {
  user: Usuario | null;
  perfil: Perfil | null;
  token: string | null;
  isAuthenticated: boolean;
  loading: boolean;
  login: (email: string, senha: string) => Promise<void>;
  logout: () => void;
};

/* =========================
   CONTEXT (EXPORTADO)
========================= */

export const AuthContext = createContext<AuthContextType | null>(null);

/* =========================
   PROVIDER
========================= */

export function AuthProvider({
  children
}: {
  children: React.ReactNode;
}) {
  const [user, setUser] = useState<Usuario | null>(null);
  const [perfil, setPerfil] = useState<Perfil | null>(null);
  const [token, setToken] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);

  /* =========================
     BOOTSTRAP (carrega /me)
  ========================= */
  useEffect(() => {
    const storedToken = localStorage.getItem("token");

    if (!storedToken) {
      setLoading(false);
      return;
    }

    setToken(storedToken);

    api
      .get<MeResponse>("/account/me")
      .then((res) => {
        setUser(res.data.usuario);
        setPerfil(res.data.perfil);
      })
      .catch(() => {
        logout();
      })
      .finally(() => {
        setLoading(false);
      });
  }, []);

  /* =========================
     LOGIN
  ========================= */
  async function login(email: string, senha: string) {
    const { data } = await api.post<LoginResponse>(
      "/usuario/login",
      {
        email,
        senha
      }
    );

    localStorage.setItem("token", data.token);
    setToken(data.token);

    const me = await api.get<MeResponse>("/account/me");

    setUser(me.data.usuario);
    setPerfil(me.data.perfil);
  }

  /* =========================
     LOGOUT
  ========================= */
  function logout() {
    setUser(null);
    setPerfil(null);
    setToken(null);
    localStorage.removeItem("token");
  }

  /* =========================
     PROVIDER VALUE
  ========================= */
  return (
    <AuthContext.Provider
      value={{
        user,
        perfil,
        token,
        loading,
        isAuthenticated: !!token && !!user,
        login,
        logout
      }}
    >
      {children}
    </AuthContext.Provider>
  );
}

/* =========================
   HOOK PERSONALIZADO
========================= */

export function useAuth() {
  const context = useContext(AuthContext);

  if (!context) {
    throw new Error(
      "useAuth deve ser usado dentro de AuthProvider"
    );
  }

  return context;
}