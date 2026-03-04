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
  Pagination,
  Avatar
} from "@mui/material";
import ArrowBackIcon from "@mui/icons-material/ArrowBack";
import AddIcon from "@mui/icons-material/Add";
import PersonIcon from "@mui/icons-material/Person";
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
  usuarioId: number;
  nome: string;
  email: string;
}

interface Disciplina {
  id: number;
  nome: string;
  descricao?: string;
  cursoNome: string;
}

export default function ProfessorTurmaDetalhe() {
  const { turmaDisciplinaId } = useParams();
  const navigate = useNavigate();

  const [loading, setLoading] = useState(true);

  const [atividades, setAtividades] = useState<Atividade[]>([]);
  const [aulas, setAulas] = useState<Aula[]>([]);
  const [alunos, setAlunos] = useState<AlunoDisciplina[]>([]);

  const [disciplina, setDisciplina] = useState<Disciplina | null>(null);

  const [tab, setTab] = useState(0);

  const [searchAluno, setSearchAluno] = useState("");
  const [pageAluno, setPageAluno] = useState(1);
  const [totalAlunos, setTotalAlunos] = useState(0);

  const pageSizeAluno = 5;

  const baseUrl = api.defaults.baseURL?.replace("/api", "");

  async function carregarDisciplina() {
    try {
      if (!turmaDisciplinaId) return;

      const tdRes = await api.get(`/turmadisciplina/${turmaDisciplinaId}`);

      const disciplinaRes = await api.get(
        `/disciplina/${tdRes.data.disciplinaId}`
      );

      setDisciplina(disciplinaRes.data);
    } catch (error) {
      console.error("Erro ao carregar disciplina:", error);
    }
  }

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
    carregarDisciplina();

    if (tab === 0) carregarAtividades();
    if (tab === 1) carregarAulas();
    if (tab === 2) carregarAlunos();

  }, [turmaDisciplinaId, tab, pageAluno, searchAluno]);

  return (
    <AppLayout>

      {/* HEADER PADRÃO */}
      <Card sx={{ mb: 4 }}>
        <CardContent>

          <Box
            display="flex"
            justifyContent="space-between"
            alignItems="center"
            flexWrap="wrap"
            gap={2}
          >

            <Box maxWidth={700}>
              <Typography variant="h4" fontWeight={700}>
                {disciplina?.nome || "Disciplina"}
              </Typography>

              {disciplina?.descricao && (
                <Typography
                  variant="body1"
                  color="text.secondary"
                  sx={{ mt: 0.5 }}
                >
                  {disciplina.descricao}
                </Typography>
              )}

              {disciplina?.cursoNome && (
                <Typography
                  variant="body2"
                  color="text.secondary"
                  sx={{ mt: 1 }}
                >
                  {disciplina.cursoNome}
                </Typography>
              )}
            </Box>

            <Button
              variant="outlined"
              startIcon={<ArrowBackIcon />}
              onClick={() => navigate("/professor/academico")}
            >
              Voltar
            </Button>

          </Box>

        </CardContent>
      </Card>


      {/* BOTÕES AÇÃO */}
      <Box display="flex" justifyContent="space-between" mb={3}>

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

      </Box>


      <Tabs value={tab} onChange={(_, v) => setTab(v)} sx={{ mb: 3 }}>
        <Tab label="Atividades" />
        <Tab label="Aulas" />
        <Tab label="Alunos" />
      </Tabs>


      {/* ATIVIDADES */}
      {tab === 0 && (
        <Card>
          <CardContent>

            {loading && (
              <Box display="flex" justifyContent="center" py={4}>
                <CircularProgress size={24} />
              </Box>
            )}

            {!loading && atividades.length === 0 && (
              <Typography color="text.secondary">
                Nenhuma atividade cadastrada.
              </Typography>
            )}

            {!loading &&
              atividades.map((atividade, index) => (
                <Box key={atividade.id}>

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

                    <Typography fontWeight={600}>
                      {atividade.titulo}
                    </Typography>

                    <Button
                      size="small"
                      variant="outlined"
                      onClick={() =>
                        navigate(`/professor/academico/${turmaDisciplinaId}/atividade/${atividade.id}`)
                      }
                    >
                      Gerenciar
                    </Button>

                  </Box>

                  {index !== atividades.length - 1 && <Divider />}

                </Box>
              ))}

          </CardContent>
        </Card>
      )}


      {/* AULAS */}
      {tab === 1 && (
        <Card>
          <CardContent>

            {loading && (
              <Box display="flex" justifyContent="center" py={4}>
                <CircularProgress size={24} />
              </Box>
            )}

            {!loading && aulas.length === 0 && (
              <Typography color="text.secondary">
                Nenhuma aula cadastrada.
              </Typography>
            )}

            {!loading &&
              aulas.map((aula, index) => (
                <Box key={aula.id}>

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

                    <Box>
                      <Typography fontWeight={600}>
                        {aula.titulo}
                      </Typography>

                      <Typography variant="body2" color="text.secondary">
                        Criado em: {new Date(aula.criadoEm).toLocaleDateString()}
                      </Typography>
                    </Box>

                    <Button
                      size="small"
                      variant="outlined"
                      onClick={() =>
                        navigate(`/professor/academico/${turmaDisciplinaId}/aula/${aula.id}`)
                      }
                    >
                      Gerenciar
                    </Button>

                  </Box>

                  {index !== aulas.length - 1 && <Divider />}

                </Box>
              ))}

          </CardContent>
        </Card>
      )}


      {/* ALUNOS */}
      {tab === 2 && (
        <Card>
          <CardContent>

            <Box display="flex" justifyContent="space-between" mb={3}>

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

                  <Box display="flex" alignItems="center" gap={2}>

                    <Avatar
                      src={`${baseUrl}/api/usuario/${aluno.usuarioId}/foto`}
                      sx={{ width: 48, height: 48 }}
                    >
                      <PersonIcon />
                    </Avatar>

                    <Box>
                      <Typography fontWeight={600}>
                        {aluno.nome}
                      </Typography>

                      <Typography variant="body2" color="text.secondary">
                        {aluno.email}
                      </Typography>
                    </Box>

                  </Box>

                  <Button
                    size="small"
                    variant="outlined"
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