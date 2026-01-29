import { Navigate } from "react-router-dom";
import { useAuth } from "../contexts/AuthContext";
import type { JSX } from "@emotion/react/jsx-dev-runtime";

type RoleGuardProps = {
  allowed: number[];
  children: JSX.Element;
};

export default function RoleGuard({ allowed, children }: RoleGuardProps) {
  const { user, loading } = useAuth();

  if (loading) {
    return null;
  }

  if (!user) {
    return <Navigate to="/login" replace />;
  }

  if (!allowed.includes(user.tipo)) {
    // usuário logado, mas sem permissão
    return <Navigate to="/login" replace />;
  }

  return children;
}
