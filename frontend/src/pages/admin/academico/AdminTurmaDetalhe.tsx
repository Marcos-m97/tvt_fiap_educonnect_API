import {
  Typography,
  Box,
  Card,
  CardContent,
  Button,
  CircularProgress,
  Divider,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField,
  MenuItem,
  Chip,
  Pagination,
  Avatar
} from "@mui/material";
import ArrowBackIcon from "@mui/icons-material/ArrowBack";
import AddIcon from "@mui/icons-material/Add";
import DeleteIcon from "@mui/icons-material/Delete";
import EditIcon from "@mui/icons-material/Edit";
import PersonIcon from "@mui/icons-material/Person";
import AppLayout from "../../../components/layout/AppLayout";
import { useNavigate, useParams } from "react-router-dom";
import { useEffect, useState } from "react";
import { api } from "../../../services/api";

interface Turma {
  id: number;
  nome: string;
  periodo: string;
  semestre: string;
  cursoId: number;
  ativo: boolean;
}

interface TurmaDisciplina {
  id: number;
  disciplinaId: number;
  disciplinaNome: string;
  professorId: number;
  professorNome: string;
}

interface Disciplina {
  id: number;
  nome: string;
}

interface Professor {
  id: number;
  nome: string;
}

interface AlunoTurma {
  alunoId: number;
  usuarioId: number; // 🔥 AGORA VEM DO BACKEND
  nome: string;
  email: string;
  status: number;
}

export default function AdminTurmaDetalhe() {
  const { id } = useParams();
  const navigate = useNavigate();

  const [turma, setTurma] = useState<Turma | null>(null);
  const [vinculos, setVinculos] = useState<TurmaDisciplina[]>([]);
  const [alunos, setAlunos] = useState<AlunoTurma[]>([]);
  const [disciplinas, setDisciplinas] = useState<Disciplina[]>([]);
  const [professores, setProfessores] = useState<Professor[]>([]);
  const [loading, setLoading] = useState(false);

  const [page, setPage] = useState(1);
  const [search, setSearch] = useState("");
  const [debouncedSearch, setDebouncedSearch] = useState("");
  const pageSize = 5;
  const [totalCount, setTotalCount] = useState(0);

  const [openModal, setOpenModal] = useState(false);
  const [disciplinaId, setDisciplinaId] = useState<number | "">("");
  const [professorId, setProfessorId] = useState<number | "">("");

  const baseUrl = api.defaults.baseURL?.replace("/api", "");

  useEffect(() => {
    const timer = setTimeout(() => {
      setDebouncedSearch(search);
    }, 500);
    return () => clearTimeout(timer);
  }, [search]);

  function getStatusLabel(status: number) {
    switch (status) {
      case 0: return { label: "Inativa", color: "default" };
      case 1: return { label: "Inscrição", color: "info" };
      case 2: return { label: "Pagamento", color: "warning" };
      case 3: return { label: "Documentos", color: "warning" };
      case 4: return { label: "Efetivada", color: "success" };
      default: return { label: "Desconhecido", color: "default" };
    }
  }

  async function carregarDados() {
    try {
      setLoading(true);

      const turmaResponse = await api.get(`/turma/${id}`);
      const vinculosResponse = await api.get(`/turmadisciplina/turma/${id}`);

      const alunosResponse = await api.get(
        `/matricula/turma/${id}/alunos`,
        {
          params: { page, pageSize, search: debouncedSearch }
        }
      );

      setTurma(turmaResponse.data);
      setVinculos(vinculosResponse.data);
      setAlunos(alunosResponse.data.items);
      setTotalCount(alunosResponse.data.totalCount);

    } catch (error) {
      console.error("Erro ao carregar turma:", error);
    } finally {
      setLoading(false);
    }
  }

  async function carregarSelects() {
    if (!turma) return;

    const disciplinasResponse = await api.get(
      `/disciplina/curso/${turma.cursoId}`
    );

    const professoresResponse = await api.get(`/professor`);

    setDisciplinas(disciplinasResponse.data);
    setProfessores(professoresResponse.data);
  }

  async function vincularDisciplina() {
    await api.post(`/turmadisciplina`, {
      turmaId: Number(id),
      disciplinaId: Number(disciplinaId),
      professorId: Number(professorId)
    });

    setOpenModal(false);
    setDisciplinaId("");
    setProfessorId("");
    carregarDados();
  }

  async function removerVinculo(vinculoId: number) {
    if (!confirm("Deseja remover esta disciplina da turma?")) return;
    await api.delete(`/turmadisciplina/${vinculoId}`);
    carregarDados();
  }

  async function desativarTurma() {
    if (!confirm("Deseja desativar esta turma?")) return;
    await api.delete(`/turma/${turma?.id}`);
    carregarDados();
  }

  async function reativarTurma() {
    await api.put(`/turma/reativar/${turma?.id}`);
    carregarDados();
  }

  useEffect(() => {
    carregarDados();
  }, [id, page, debouncedSearch]);

  useEffect(() => {
    if (openModal) carregarSelects();
  }, [openModal]);

  if (!turma) {
    return (
      <AppLayout>
        <Typography>Turma não encontrada.</Typography>
      </AppLayout>
    );
  }

  return (
    <AppLayout>

      {/* HEADER */}
      <Box mb={4} display="flex" justifyContent="space-between" alignItems="flex-start">
        <Box>
          <Typography variant="h4">{turma.nome}</Typography>
          <Typography color="text.secondary">
            {turma.semestre} • {turma.periodo}
          </Typography>
          {!turma.ativo && (
            <Chip label="Inativa" color="error" sx={{ mt: 1 }} />
          )}
        </Box>

        <Box display="flex" gap={2}>
          <Button
            variant="outlined"
            startIcon={<EditIcon />}
            onClick={() =>
              navigate(`/admin/academico/turmas/${turma.id}/editar`)
            }
          >
            Editar
          </Button>

          <Button
            variant="outlined"
            startIcon={<ArrowBackIcon />}
            onClick={() => navigate(-1)}
          >
            Voltar
          </Button>
        </Box>
      </Box>

      <Box mb={4}>
        {turma.ativo ? (
          <Button variant="contained" color="error" onClick={desativarTurma}>
            Desativar Turma
          </Button>
        ) : (
          <Button variant="contained" color="success" onClick={reativarTurma}>
            Reativar Turma
          </Button>
        )}
      </Box>

      <Divider sx={{ mb: 4 }} />

      {/* DISCIPLINAS */}
      <Card>
        <CardContent>
          <Box display="flex" justifyContent="space-between" alignItems="center" mb={3}>
            <Typography variant="h6">
              Disciplinas da Turma
            </Typography>

            <Button
              size="small"
              startIcon={<AddIcon />}
              disabled={!turma.ativo}
              onClick={() => setOpenModal(true)}
            >
              Vincular Disciplina
            </Button>
          </Box>

          {vinculos.length === 0 && (
            <Typography variant="body2" color="text.secondary">
              Nenhuma disciplina vinculada.
            </Typography>
          )}

          {vinculos.map((v) => (
            <Box key={v.id} py={1.5} display="flex" justifyContent="space-between" alignItems="center">
              <Box>
                <Typography fontWeight={600}>{v.disciplinaNome}</Typography>
                <Typography variant="body2" color="text.secondary">
                  Professor: {v.professorNome}
                </Typography>
              </Box>

              <Button
                size="small"
                color="error"
                startIcon={<DeleteIcon />}
                disabled={!turma.ativo}
                onClick={() => removerVinculo(v.id)}
              >
                Remover
              </Button>
            </Box>
          ))}
        </CardContent>
      </Card>

      {/* ALUNOS */}
      <Card sx={{ mt: 4 }}>
        <CardContent>

          <Box display="flex" justifyContent="space-between" alignItems="center" mb={3}>
            <Typography variant="h6">
              Alunos da Turma
            </Typography>

            <TextField
              size="small"
              placeholder="Buscar aluno..."
              value={search}
              onChange={(e) => {
                setSearch(e.target.value);
                setPage(1);
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

          {!loading && alunos.map((aluno) => {
            const statusInfo = getStatusLabel(aluno.status);

            return (
              <Box
                key={aluno.alunoId}
                py={1.5}
                display="flex"
                justifyContent="space-between"
                alignItems="center"
              >
                <Box display="flex" alignItems="center" gap={2}>

                  <Avatar
                    src={`${baseUrl}/api/usuario/${aluno.usuarioId}/foto`}
                    sx={{ width: 48, height: 48 }}
                  >
                    <PersonIcon />
                  </Avatar>

                  <Box>
                    <Typography fontWeight={600}>{aluno.nome}</Typography>
                    <Typography variant="body2" color="text.secondary">
                      {aluno.email}
                    </Typography>
                  </Box>
                </Box>

                <Box display="flex" gap={2} alignItems="center">
                  <Chip
                    label={statusInfo.label}
                    color={statusInfo.color as any}
                    size="small"
                  />

                  <Button
                    size="small"
                    variant="outlined"
                    onClick={() =>
                      navigate(`/admin/academico/alunos/${aluno.alunoId}`, {
                        state: { nome: aluno.nome }
                      })
                    }
                  >
                    Ver Detalhes
                  </Button>
                </Box>
              </Box>
            );
          })}

          {totalCount > pageSize && (
            <Box display="flex" justifyContent="center" mt={3}>
              <Pagination
                count={Math.ceil(totalCount / pageSize)}
                page={page}
                onChange={(_, value) => setPage(value)}
                color="primary"
              />
            </Box>
          )}

        </CardContent>
      </Card>

      {/* MODAL */}
      <Dialog open={openModal} onClose={() => setOpenModal(false)} fullWidth>
        <DialogTitle>Vincular Disciplina</DialogTitle>
        <DialogContent sx={{ display: "flex", flexDirection: "column", gap: 3, mt: 2 }}>
          <TextField
            select
            label="Disciplina"
            value={disciplinaId}
            onChange={(e) => setDisciplinaId(Number(e.target.value))}
            fullWidth
          >
            {disciplinas.map((d) => (
              <MenuItem key={d.id} value={d.id}>
                {d.nome}
              </MenuItem>
            ))}
          </TextField>

          <TextField
            select
            label="Professor"
            value={professorId}
            onChange={(e) => setProfessorId(Number(e.target.value))}
            fullWidth
          >
            {professores.map((p) => (
              <MenuItem key={p.id} value={p.id}>
                {p.nome}
              </MenuItem>
            ))}
          </TextField>
        </DialogContent>

        <DialogActions>
          <Button onClick={() => setOpenModal(false)}>
            Cancelar
          </Button>

          <Button
            variant="contained"
            onClick={vincularDisciplina}
            disabled={!disciplinaId || !professorId}
          >
            Vincular
          </Button>
        </DialogActions>
      </Dialog>

    </AppLayout>
  );
}