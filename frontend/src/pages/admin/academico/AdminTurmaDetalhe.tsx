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
  Chip
} from "@mui/material";
import ArrowBackIcon from "@mui/icons-material/ArrowBack";
import AddIcon from "@mui/icons-material/Add";
import DeleteIcon from "@mui/icons-material/Delete";
import EditIcon from "@mui/icons-material/Edit";
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

export default function AdminTurmaDetalhe() {
  const { id } = useParams();
  const navigate = useNavigate();

  const [turma, setTurma] = useState<Turma | null>(null);
  const [vinculos, setVinculos] = useState<TurmaDisciplina[]>([]);
  const [disciplinas, setDisciplinas] = useState<Disciplina[]>([]);
  const [professores, setProfessores] = useState<Professor[]>([]);
  const [loading, setLoading] = useState(false);

  const [openModal, setOpenModal] = useState(false);
  const [disciplinaId, setDisciplinaId] = useState<number | "">("");
  const [professorId, setProfessorId] = useState<number | "">("");

  async function carregarDados() {
    try {
      setLoading(true);

      const turmaResponse = await api.get(`/turma/${id}`);
      const vinculosResponse = await api.get(`/turmadisciplina/turma/${id}`);

      setTurma(turmaResponse.data);
      setVinculos(vinculosResponse.data);

    } catch (error) {
      console.error("Erro ao carregar turma:", error);
    } finally {
      setLoading(false);
    }
  }

  async function desativarTurma() {
    if (!confirm("Deseja desativar esta turma?")) return;

    try {
      await api.delete(`/turma/${turma?.id}`);
      carregarDados();
    } catch (error) {
      console.error("Erro ao desativar turma:", error);
    }
  }

  async function reativarTurma() {
    try {
      await api.put(`/turma/reativar/${turma?.id}`);
      carregarDados();
    } catch (error) {
      console.error("Erro ao reativar turma:", error);
    }
  }

  useEffect(() => {
    carregarDados();
  }, [id]);

  if (loading) {
    return (
      <AppLayout>
        <Box display="flex" justifyContent="center" py={6}>
          <CircularProgress />
        </Box>
      </AppLayout>
    );
  }

  if (!turma) {
    return (
      <AppLayout>
        <Typography>Turma não encontrada.</Typography>
      </AppLayout>
    );
  }

  return (
    <AppLayout>

      {/* HEADER PADRONIZADO */}
      <Box
        mb={4}
        display="flex"
        justifyContent="space-between"
        alignItems="flex-start"
      >
        <Box>
          <Typography variant="h4">
            {turma.nome}
          </Typography>

          <Typography color="text.secondary">
            {turma.semestre} • {turma.periodo}
          </Typography>
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

      <Box mt={2} mb={4}>
        {turma.ativo ? (
          <Button
            variant="contained"
            color="error"
            onClick={desativarTurma}
          >
            Desativar Turma
          </Button>
        ) : (
          <Button
            variant="contained"
            color="success"
            onClick={reativarTurma}
          >
            Reativar Turma
          </Button>
        )}
      </Box>

      <Divider sx={{ mb: 4 }} />

      {/* DISCIPLINAS */}
      <Card>
        <CardContent>

          <Box
            display="flex"
            justifyContent="space-between"
            alignItems="center"
            mb={3}
          >
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
            <Box
              key={v.id}
              py={1.5}
              display="flex"
              justifyContent="space-between"
              alignItems="center"
            >
              <Box>
                <Typography fontWeight={600}>
                  {v.disciplinaNome}
                </Typography>
                <Typography variant="body2" color="text.secondary">
                  Professor: {v.professorNome}
                </Typography>
              </Box>

              <Button
                size="small"
                color="error"
                startIcon={<DeleteIcon />}
                disabled={!turma.ativo}
              >
                Remover
              </Button>
            </Box>
          ))}

        </CardContent>
      </Card>

    </AppLayout>
  );
}