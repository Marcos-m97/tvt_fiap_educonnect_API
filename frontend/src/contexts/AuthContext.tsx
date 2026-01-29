import { createContext, useContext, useEffect, useState } from "react";
import { api } from "../services/api";

type Usuario = {
  id: number;
  nome: string;
  tipo: number; // 1=Admin | 2=Professor | 3=Aluno
};

type LoginResponse = {
  token: string;
  usuario: Usuario;
};

type AuthContextType = {
  user: Usuario | null;
  token: string | null;
  isAuthenticated: boolean;
  login: (email: string, senha: string) => Promise<Usuario>;
  logout: () => void;
};

const AuthContext = createContext<AuthContextType | null>(null);

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [user, setUser] = useState<Usuario | null>(null);
  const [token, setToken] = useState<string | null>(null);

  // 🔄 Recupera sessão ao recarregar a página
  useEffect(() => {
    const storedToken = localStorage.getItem("token");
    const storedUser = localStorage.getItem("user");

    if (storedToken && storedUser) {
      setToken(storedToken);
      setUser(JSON.parse(storedUser));
    }
  }, []);

  // 🔐 Login
  async function login(email: string, senha: string) {
    const { data } = await api.post<LoginResponse>("/usuario/login", {
      email,
      senha,
    });

    setToken(data.token);
    setUser(data.usuario);

    localStorage.setItem("token", data.token);
    localStorage.setItem("user", JSON.stringify(data.usuario));

    return data.usuario;
  }

  // 🚪 Logout
  function logout() {
    setToken(null);
    setUser(null);
    localStorage.removeItem("token");
    localStorage.removeItem("user");
  }

  return (
    <AuthContext.Provider
      value={{
        user,
        token,
        isAuthenticated: !!token,
        login,
        logout,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const ctx = useContext(AuthContext);
  if (!ctx) {
    throw new Error("useAuth deve ser usado dentro de AuthProvider");
  }
  return ctx;
}
