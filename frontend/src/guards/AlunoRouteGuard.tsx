import { useEffect, useState } from "react";
import { Navigate } from "react-router-dom";
import { api } from "../services/api";
import CircularProgress from "@mui/material/CircularProgress";
import Box from "@mui/material/Box";

interface Props {
  children: React.ReactNode;
}

export default function AlunoRouteGuard({ children }: Props) {
  const [loading, setLoading] = useState(true);
  const [permitido, setPermitido] = useState(false);

  useEffect(() => {
    async function verificar() {
      try {
        const contexto = await api.get("/account/me/contexto");

        const matricula = await api.get(
          `/matricula/aluno/${contexto.data.alunoId}`
        );

        if (
          !matricula.data ||
          matricula.data.length === 0 ||
          matricula.data[0].status !== 4
        ) {
          setPermitido(false);
        } else {
          setPermitido(true);
        }
      } catch {
        setPermitido(false);
      } finally {
        setLoading(false);
      }
    }

    verificar();
  }, []);

  if (loading) {
    return (
      <Box display="flex" justifyContent="center" mt={10}>
        <CircularProgress />
      </Box>
    );
  }

  if (!permitido) {
    return <Navigate to="/aluno/matricula" replace />;
  }

  return <>{children}</>;
}