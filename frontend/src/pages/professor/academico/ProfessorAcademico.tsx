import {
  Typography,
  Box,
  Card,
  CardContent,
  Button,
  Divider,
  CircularProgress,
  Chip
} from "@mui/material";
import ArrowBackIcon from "@mui/icons-material/ArrowBack";
import SchoolIcon from "@mui/icons-material/School";
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

      {/* HEADER PADRÃO */}
      <Card sx={{ mb: 4 }}>
        <CardContent>

          <Box textAlign="center" maxWidth={650} mx="auto">

            <Typography
              variant="h4"
              fontWeight={700}
              gutterBottom
            >
              Gestão Acadêmica
            </Typography>

            <Typography
              variant="body1"
              color="text.secondary"
              sx={{ lineHeight: 1.7 }}
            >
              Gerencie suas disciplinas vinculadas às turmas.
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
              onClick={() => navigate("/professor")}
            >
              Voltar
            </Button>
          </Box>

        </CardContent>
      </Card>


      {/* LISTA */}
      <Card>
        <CardContent>

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

                  {/* INFO */}
               <Box display="flex" alignItems="center" gap={1.5}>

  <SchoolIcon
    fontSize="small"
    color="primary"
  />

  <Typography fontWeight={600}>
    {td.disciplinaNome}
  </Typography>

  <Chip
    label={td.turmaNome}
    size="small"
    color="primary"
    variant="outlined"
  />

</Box>

                  {/* BOTÃO */}
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