import {
  Typography,
  Box,
  Card,
  CardContent,
  Button,
  Divider,
  CircularProgress,
  Tabs,
  Tab
} from "@mui/material";
import ArrowBackIcon from "@mui/icons-material/ArrowBack";
import AddIcon from "@mui/icons-material/Add";
import AppLayout from "../../../components/layout/AppLayout";
import { useEffect, useState } from "react";
import { api } from "../../../services/api";
import { useNavigate, useParams } from "react-router-dom";

interface Atividade {
  id: number;
  titulo: string;
  descricao: string;
  dataEntrega?: string;
}

interface Aula {
  id: number;
  turmaDisciplinaId: number;
  titulo: string;
  descricao: string;
  urlVideo?: string;
  materialApoio?: string;
  criadoEm: string;
}

export default function ProfessorTurmaDetalhe() {
  const { turmaDisciplinaId } = useParams();
  const navigate = useNavigate();

  const [loading, setLoading] = useState(true);
  const [atividades, setAtividades] = useState<Atividade[]>([]);
  const [aulas, setAulas] = useState<Aula[]>([]);
  const [tab, setTab] = useState(0);

  async function carregarAtividades() {
    try {
      setLoading(true);

      const response = await api.get(
        `/atividade/turma-disciplina/${turmaDisciplinaId}`
      );

      setAtividades(response.data);
    } catch (error) {
      console.error("Erro ao carregar atividades:", error);
    } finally {
      setLoading(false);
    }
  }

  async function carregarAulas() {
    try {
      setLoading(true);

      const response = await api.get(
        `/aulas/turma-disciplina/${turmaDisciplinaId}`
      );

      setAulas(response.data);

    } catch (error) {
      console.error("Erro ao carregar aulas:", error);
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    if (tab === 0) {
      carregarAtividades();
    }

    if (tab === 1) {
      carregarAulas();
    }
  }, [turmaDisciplinaId, tab]);

  return (
    <AppLayout>

      {/* HEADER */}
      <Box textAlign="center" mb={3}>
        <Typography variant="h3" fontWeight={700} gutterBottom>
          Gestão da Disciplina
        </Typography>

        <Typography variant="body1" color="text.secondary">
          Gerencie atividades, aulas e alunos.
        </Typography>
      </Box>

      {/* BOTÕES */}
      <Box
        display="flex"
        justifyContent="space-between"
        alignItems="center"
        mb={3}
      >
        {tab === 0 && (
          <Button
            variant="contained"
            startIcon={<AddIcon />}
            onClick={() =>
              navigate(
                `/professor/academico/${turmaDisciplinaId}/nova-atividade`
              )
            }
          >
            Criar Atividade
          </Button>
        )}

        {tab === 1 && (
          <Button
            variant="contained"
            startIcon={<AddIcon />}
            onClick={() =>
              navigate(
                `/professor/academico/${turmaDisciplinaId}/nova-aula`
              )
            }
          >
            Criar Aula
          </Button>
        )}

        <Button
          variant="outlined"
          startIcon={<ArrowBackIcon />}
          onClick={() => navigate("/professor/academico")}
        >
          Voltar
        </Button>
      </Box>

      <Divider sx={{ mb: 3 }} />

      {/* TABS */}
      <Tabs
        value={tab}
        onChange={(_, newValue) => setTab(newValue)}
        sx={{ mb: 3 }}
      >
        <Tab label="Atividades" />
        <Tab label="Aulas" />
        <Tab label="Alunos" />
      </Tabs>

      {/* ========================= */}
      {/* ABA ATIVIDADES */}
      {/* ========================= */}
      {tab === 0 && (
        <Card sx={{ borderRadius: 4 }}>
          <CardContent>

            {loading && (
              <Box display="flex" justifyContent="center" py={4}>
                <CircularProgress size={24} />
              </Box>
            )}

            {!loading && atividades.length === 0 && (
              <Typography variant="body2" color="text.secondary">
                Nenhuma atividade cadastrada.
              </Typography>
            )}

            {!loading &&
              atividades.map((atividade) => (
                <Box key={atividade.id} mb={3}>
                  <Box
                    display="flex"
                    justifyContent="space-between"
                    alignItems="center"
                  >
                    <Box maxWidth="80%">
                      <Typography fontWeight={600}>
                        {atividade.titulo}
                      </Typography>

                      {atividade.dataEntrega && (
                        <Typography
                          variant="body2"
                          color="text.secondary"
                          mt={1}
                        >
                          Entrega: {atividade.dataEntrega}
                        </Typography>
                      )}
                    </Box>

                    <Button
                      size="small"
                      variant="outlined"
                      onClick={() =>
                        navigate(
                          `/professor/academico/${turmaDisciplinaId}/atividade/${atividade.id}`
                        )
                      }
                    >
                      Gerenciar
                    </Button>
                  </Box>
                </Box>
              ))}

          </CardContent>
        </Card>
      )}

      {/* ========================= */}
      {/* ABA AULAS */}
      {/* ========================= */}
      {tab === 1 && (
        <Card sx={{ borderRadius: 4 }}>
          <CardContent>

            {loading && (
              <Box display="flex" justifyContent="center" py={4}>
                <CircularProgress size={24} />
              </Box>
            )}

            {!loading && aulas.length === 0 && (
              <Typography variant="body2" color="text.secondary">
                Nenhuma aula cadastrada.
              </Typography>
            )}

            {!loading &&
              aulas.map((aula) => (
                <Box key={aula.id} mb={3}>
                  <Box
                    display="flex"
                    justifyContent="space-between"
                    alignItems="center"
                  >
                    <Box maxWidth="80%">
                      <Typography fontWeight={600}>
                        {aula.titulo}
                      </Typography>

                      <Typography
                        variant="body2"
                        color="text.secondary"
                        mt={1}
                      >
                        Criado em: {new Date(aula.criadoEm).toLocaleDateString()}
                      </Typography>
                    </Box>

                    <Button
                      size="small"
                      variant="outlined"
                      onClick={() =>
                        navigate(
                          `/professor/academico/${turmaDisciplinaId}/aula/${aula.id}`
                        )
                      }
                    >
                      Gerenciar
                    </Button>
                  </Box>
                </Box>
              ))}

          </CardContent>
        </Card>
      )}

      {/* ========================= */}
      {/* ABA ALUNOS */}
      {/* ========================= */}
      {tab === 2 && (
        <Card sx={{ borderRadius: 4 }}>
          <CardContent>
            <Typography variant="body2" color="text.secondary">
              Lista de alunos da turma em construção.
            </Typography>
          </CardContent>
        </Card>
      )}

    </AppLayout>
  );
}