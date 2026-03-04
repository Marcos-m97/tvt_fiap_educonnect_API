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

      {/* HEADER PADRÃO */}
      <Card sx={{ mb: 4 }}>
        <CardContent>

          <Box textAlign="center" maxWidth={650} mx="auto">

            <Typography
              variant="h4"
              fontWeight={700}
              gutterBottom
            >
              Área Acadêmica
            </Typography>

            <Typography
              variant="body1"
              color="text.secondary"
              sx={{ lineHeight: 1.7 }}
            >
              Visualize suas disciplinas, aulas e atividades.
            </Typography>

          </Box>

          {/* AÇÃO */}
          <Box
            display="flex"
            justifyContent="center"
            alignItems="center"
            mt={3}
          >
            <Button
              variant="outlined"
              startIcon={<ArrowBackIcon />}
              onClick={() => navigate("/aluno")}
            >
              Voltar
            </Button>
          </Box>

        </CardContent>
      </Card>


      {/* LISTA DE DISCIPLINAS */}
      <Card>
        <CardContent>

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
                  py={2}
                  sx={{
                    transition: "0.25s",
                    "&:hover": {
                      background: "rgba(0,0,0,0.03)",
                      borderRadius: 2,
                      px: 1
                    }
                  }}
                >

                  <Typography
                    fontWeight={600}
                  >
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