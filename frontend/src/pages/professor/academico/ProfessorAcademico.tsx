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

interface TurmaDisciplina {
  turmaDisciplinaId: number;
  turmaId: number;
  turmaNome: string;
  disciplinaId: number;
  disciplinaNome: string;
}

export default function ProfessorAcademico() {
  const [loading, setLoading] = useState(true);
  const [turmasDisciplinas, setTurmasDisciplinas] = useState<TurmaDisciplina[]>([]);
  const navigate = useNavigate();

  useEffect(() => {
    async function fetchContexto() {
      try {
        const response = await api.get("/account/me/contexto");
        setTurmasDisciplinas(response.data.turmasDisciplinas || []);
      } catch (error) {
        console.error("Erro ao carregar disciplinas:", error);
      } finally {
        setLoading(false);
      }
    }

    fetchContexto();
  }, []);

  return (
    <AppLayout>

      {/* HEADER CENTRALIZADO */}
      <Box textAlign="center" mb={3}>
        <Typography variant="h3" fontWeight={700} gutterBottom>
          Gestão Acadêmica
        </Typography>

        <Typography
          variant="body1"
          color="text.secondary"
          sx={{ fontSize: 16 }}
        >
          Gerencie suas disciplinas vinculadas às turmas.
        </Typography>
      </Box>

      {/* BOTÃO VOLTAR */}
      <Box display="flex" justifyContent="flex-end" mb={3}>
        <Button
          variant="outlined"
          startIcon={<ArrowBackIcon />}
          onClick={() => navigate("/professor")}
          sx={{ textTransform: "none" }}
        >
          Voltar
        </Button>
      </Box>

      <Divider sx={{ mb: 4 }} />

      {/* LISTA */}
      <Card sx={{ borderRadius: 4 }}>
        <CardContent sx={{ px: 4 }}>

          {loading && (
            <Box display="flex" justifyContent="center" py={4}>
              <CircularProgress size={24} />
            </Box>
          )}

          {!loading && turmasDisciplinas.length === 0 && (
            <Typography variant="body2" color="text.secondary">
              Nenhuma disciplina vinculada encontrada.
            </Typography>
          )}

          {!loading &&
            turmasDisciplinas.map((td, index) => (
              <Box key={td.turmaDisciplinaId}>
                <Box
                  display="flex"
                  justifyContent="space-between"
                  alignItems="center"
                  py={3}
                >
                  <Box display="flex" alignItems="center" gap={2}>
                    <Typography
                      variant="h6"
                      fontWeight={700}
                    >
                      {td.disciplinaNome}
                    </Typography>

                    {/* Turma estilo retangular */}
                    <Box
                      sx={{
                        px: 1.5,
                        py: 0.5,
                        fontSize: 12,
                        fontWeight: 600,
                        borderRadius: "4px",
                        backgroundColor: "primary.main",
                        color: "white"
                      }}
                    >
                      {td.turmaNome}
                    </Box>
                  </Box>

                  <Button
                    size="small"
                    variant="outlined"
                    onClick={() =>
                      navigate(`/professor/turma/${td.turmaDisciplinaId}`)
                    }
                  >
                    Gerenciar
                  </Button>
                </Box>

                {index !== turmasDisciplinas.length - 1 && <Divider />}
              </Box>
            ))}

        </CardContent>
      </Card>

    </AppLayout>
  );
}