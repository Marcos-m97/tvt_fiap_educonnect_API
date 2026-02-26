import {
  Typography,
  Box,
  Card,
  CardContent,
  Button,
  Divider,
  CircularProgress,
  Tabs,
  Tab,
  TextField,
  Pagination
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

interface AlunoDisciplina {
  alunoId: number;
  nome: string;
  email: string;
}

export default function ProfessorTurmaDetalhe() {
  const { turmaDisciplinaId } = useParams();
  const navigate = useNavigate();

  const [loading, setLoading] = useState(true);
  const [atividades, setAtividades] = useState<Atividade[]>([]);
  const [aulas, setAulas] = useState<Aula[]>([]);
  const [alunos, setAlunos] = useState<AlunoDisciplina[]>([]);
  const [tab, setTab] = useState(0);

  // Alunos
  const [searchAluno, setSearchAluno] = useState("");
  const [pageAluno, setPageAluno] = useState(1);
  const [totalAlunos, setTotalAlunos] = useState(0);
  const pageSizeAluno = 5;

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

  async function carregarAlunos() {
    try {
      setLoading(true);

      if (!turmaDisciplinaId) return;

      const turmaDisciplinaResponse = await api.get(
        `/turmadisciplina/${turmaDisciplinaId}`
      );

      const turmaId = turmaDisciplinaResponse.data.turmaId;

      const response = await api.get(
        `/matricula/turma/${turmaId}/alunos`,
        {
          params: {
            page: pageAluno,
            pageSize: pageSizeAluno,
            search: searchAluno
          }
        }
      );

      setAlunos(response.data.items);
      setTotalAlunos(response.data.totalCount);

    } catch (error) {
      console.error("Erro ao carregar alunos:", error);
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    if (tab === 0) carregarAtividades();
    if (tab === 1) carregarAulas();
    if (tab === 2) carregarAlunos();
  }, [turmaDisciplinaId, tab, pageAluno, searchAluno]);

  return (
    <AppLayout>

      <Box textAlign="center" mb={3}>
        <Typography variant="h3" fontWeight={700} gutterBottom>
          Gestão da Disciplina
        </Typography>
        <Typography variant="body1" color="text.secondary">
          Gerencie atividades, aulas e alunos.
        </Typography>
      </Box>

<Box
  display="flex"
  justifyContent={tab === 2 ? "flex-end" : "space-between"}
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

      <Tabs value={tab} onChange={(_, v) => setTab(v)} sx={{ mb: 3 }}>
        <Tab label="Atividades" />
        <Tab label="Aulas" />
        <Tab label="Alunos" />
      </Tabs>

      {/* ATIVIDADES */}
      {tab === 0 && (
        <Card sx={{ borderRadius: 1, boxShadow: 2, p: 3 }}>
          <CardContent sx={{ p: 0 }}>
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
                <Box
                  key={atividade.id}
                  py={2}
                  display="flex"
                  justifyContent="space-between"
                  alignItems="center"
                  borderBottom="1px solid #eee"
                >
                  <Box pr={2} maxWidth="80%">
                    <Typography fontWeight={600}>
                      {atividade.titulo}
                    </Typography>
                  </Box>

                  <Button
                    size="small"
                    variant="outlined"
                    sx={{ minWidth: 110 }}
                    onClick={() =>
                      navigate(`/professor/academico/${turmaDisciplinaId}/atividade/${atividade.id}`)
                    }
                  >
                    Gerenciar
                  </Button>
                </Box>
              ))}
          </CardContent>
        </Card>
      )}

      {/* AULAS */}
      {tab === 1 && (
        <Card sx={{ borderRadius: 1, boxShadow: 2, p: 3 }}>
          <CardContent sx={{ p: 0 }}>
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
                <Box
                  key={aula.id}
                  py={2}
                  display="flex"
                  justifyContent="space-between"
                  alignItems="center"
                  borderBottom="1px solid #eee"
                >
                  <Box pr={2} maxWidth="80%">
                    <Typography fontWeight={600}>
                      {aula.titulo}
                    </Typography>

                    <Typography variant="body2" color="text.secondary" mt={1}>
                      Criado em: {new Date(aula.criadoEm).toLocaleDateString()}
                    </Typography>
                  </Box>

                  <Button
                    size="small"
                    variant="outlined"
                    sx={{ minWidth: 110 }}
                    onClick={() =>
                      navigate(`/professor/academico/${turmaDisciplinaId}/aula/${aula.id}`)
                    }
                  >
                    Gerenciar
                  </Button>
                </Box>
              ))}
          </CardContent>
        </Card>
      )}

      {/* ALUNOS */}
      {tab === 2 && (
        <Card sx={{ borderRadius: 1, boxShadow: 2, p: 3 }}>
          <CardContent sx={{ p: 0 }}>

            <Box display="flex" justifyContent="space-between" alignItems="center" mb={3}>
              <Typography variant="h6">
                Alunos da Disciplina
              </Typography>

              <TextField
                size="small"
                placeholder="Buscar aluno..."
                value={searchAluno}
                onChange={(e) => {
                  setSearchAluno(e.target.value);
                  setPageAluno(1);
                }}
              />
            </Box>

            {loading && (
              <Box display="flex" justifyContent="center" py={3}>
                <CircularProgress size={24} />
              </Box>
            )}

            {!loading && alunos.length === 0 && (
              <Typography variant="body2" color="text.secondary">
                Nenhum aluno encontrado.
              </Typography>
            )}

            {!loading &&
              alunos.map((aluno) => (
                <Box
                  key={aluno.alunoId}
                  py={2}
                  display="flex"
                  justifyContent="space-between"
                  alignItems="center"
                  borderBottom="1px solid #eee"
                >
                  <Box>
                    <Typography fontWeight={600}>
                      {aluno.nome}
                    </Typography>
                    <Typography variant="body2" color="text.secondary">
                      {aluno.email}
                    </Typography>
                  </Box>

                  <Button
                    size="small"
                    variant="outlined"
                    sx={{ minWidth: 130 }}
                    onClick={() =>
                      navigate(
                        `/professor/academico/${turmaDisciplinaId}/aluno/${aluno.alunoId}`,
                        { state: { nome: aluno.nome } }
                      )
                    }
                  >
                    Ver Desempenho
                  </Button>
                </Box>
              ))}

            {totalAlunos > pageSizeAluno && (
              <Box display="flex" justifyContent="center" mt={3}>
                <Pagination
                  count={Math.ceil(totalAlunos / pageSizeAluno)}
                  page={pageAluno}
                  onChange={(_, value) => setPageAluno(value)}
                />
              </Box>
            )}

          </CardContent>
        </Card>
      )}

    </AppLayout>
  );
}