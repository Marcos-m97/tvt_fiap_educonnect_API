import {
  Typography,
  Box,
  Card,
  CardContent,
  Button,
  Divider,
  CircularProgress
} from "@mui/material";
import ArrowBackIcon from "@mui/icons-material/ArrowBack";
import AppLayout from "../../../components/layout/AppLayout";
import { useEffect, useState } from "react";
import { api } from "../../../services/api";
import { useNavigate } from "react-router-dom";

interface ContextoResponse {
  tipoUsuario: string;
  alunoId: number;
  turmaId: number;
  turmaNome: string;
  cursoNome: string;
  disciplinas: {
    disciplinaId: number;
    nome: string;
  }[];
}

export default function AlunoAcademico() {
  const [loading, setLoading] = useState(true);
  const [disciplinas, setDisciplinas] = useState<
    { disciplinaId: number; nome: string }[]
  >([]);

  const navigate = useNavigate();

  useEffect(() => {
    async function fetchContexto() {
      try {
        const response = await api.get<ContextoResponse>(
          "/account/me/contexto"
        );

        setDisciplinas(response.data.disciplinas || []);
      } catch (error) {
        console.error("Erro ao carregar disciplinas do aluno:", error);
      } finally {
        setLoading(false);
      }
    }

    fetchContexto();
  }, []);

  return (
    <AppLayout>
      {/* HEADER */}
      <Box textAlign="center" mb={3}>
        <Typography variant="h3" fontWeight={700} gutterBottom>
          Área Acadêmica
        </Typography>

        <Typography
          variant="body1"
          color="text.secondary"
          sx={{ fontSize: 16 }}
        >
          Visualize suas disciplinas, aulas e atividades.
        </Typography>
      </Box>

      {/* BOTÃO VOLTAR */}
      <Box display="flex" justifyContent="flex-end" mb={3}>
        <Button
          variant="outlined"
          startIcon={<ArrowBackIcon />}
          onClick={() => navigate("/aluno")}
          sx={{ textTransform: "none" }}
        >
          Voltar
        </Button>
      </Box>

      <Divider sx={{ mb: 4 }} />

      {/* LISTA DE DISCIPLINAS */}
      <Card sx={{ borderRadius: 4 }}>
        <CardContent sx={{ px: 4 }}>

          {loading && (
            <Box display="flex" justifyContent="center" py={4}>
              <CircularProgress size={24} />
            </Box>
          )}

          {!loading && disciplinas.length === 0 && (
            <Typography variant="body2" color="text.secondary">
              Nenhuma disciplina encontrada.
            </Typography>
          )}

          {!loading &&
            disciplinas.map((disciplina, index) => (
              <Box key={disciplina.disciplinaId}>
                <Box
                  display="flex"
                  justifyContent="space-between"
                  alignItems="center"
                  py={3}
                >
                  <Typography variant="h6" fontWeight={700}>
                    {disciplina.nome}
                  </Typography>

                  <Button
                    size="small"
                    variant="outlined"
                    onClick={() =>
                      navigate(
                        `/aluno/disciplina/${disciplina.disciplinaId}`
                      )
                    }
                  >
                    Acessar
                  </Button>
                </Box>

                {index !== disciplinas.length - 1 && <Divider />}
              </Box>
            ))}

        </CardContent>
      </Card>
    </AppLayout>
  );
}