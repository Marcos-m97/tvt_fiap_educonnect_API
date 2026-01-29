import { Navigate } from "react-router-dom";
import { useAuth } from "../contexts/AuthContext";
import type { JSX } from "@emotion/react/jsx-dev-runtime";

type AuthGuardProps = {
  children: JSX.Element;
};

export default function AuthGuard({ children }: AuthGuardProps) {
  const { user, loading } = useAuth();

  // Enquanto valida sessão (ex: token no localStorage)
  if (loading) {
    return null; // ou spinner depois
  }

  // Não logado → login
  if (!user) {
    return <Navigate to="/login" replace />;
  }

  return children;
}
