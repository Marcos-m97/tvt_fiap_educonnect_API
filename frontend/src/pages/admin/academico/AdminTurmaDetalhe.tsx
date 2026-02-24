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
  MenuItem
} from "@mui/material";
import ArrowBackIcon from "@mui/icons-material/ArrowBack";
import AddIcon from "@mui/icons-material/Add";
import DeleteIcon from "@mui/icons-material/Delete";
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

  async function carregarSelects() {
    try {
      // Disciplinas do curso da turma
      const disciplinasResponse = await api.get(
        `/disciplina/curso/${turma?.cursoId}`
      );

      // Professores (AGORA CORRETO)
      const professoresResponse = await api.get(`/professor`);

      setDisciplinas(disciplinasResponse.data);
      setProfessores(professoresResponse.data);

    } catch (error) {
      console.error("Erro ao carregar selects:", error);
    }
  }

  async function vincularDisciplina() {
    try {
      await api.post(`/turmadisciplina`, {
        turmaId: Number(id),
        disciplinaId: Number(disciplinaId),
        professorId: Number(professorId)
      });

      setOpenModal(false);
      setDisciplinaId("");
      setProfessorId("");

      carregarDados();
    } catch (error) {
      console.error("Erro ao vincular disciplina:", error);
    }
  }

  async function removerVinculo(vinculoId: number) {
    if (!confirm("Deseja remover esta disciplina da turma?"))
      return;

    try {
      await api.delete(`/turmadisciplina/${vinculoId}`);
      carregarDados();
    } catch (error) {
      console.error("Erro ao remover vínculo:", error);
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

      {/* BOTÃO VOLTAR */}
      <Box mb={3}>
        <Button
          variant="outlined"
          startIcon={<ArrowBackIcon />}
          onClick={() => navigate(-1)}
        >
          Voltar
        </Button>
      </Box>

      {/* HEADER */}
      <Box mb={4}>
        <Typography variant="h4">{turma.nome}</Typography>
        <Typography color="text.secondary">
          {turma.semestre} • {turma.periodo}
        </Typography>
      </Box>

      <Divider sx={{ mb: 4 }} />

      {/* DISCIPLINAS DA TURMA */}
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
              onClick={() => {
                setOpenModal(true);
                carregarSelects();
              }}
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
                onClick={() => removerVinculo(v.id)}
              >
                Remover
              </Button>
            </Box>
          ))}

        </CardContent>
      </Card>

      {/* MODAL VINCULAR */}
      <Dialog
        open={openModal}
        onClose={() => setOpenModal(false)}
        fullWidth
        maxWidth="md"
      >
        <DialogTitle>Vincular Disciplina</DialogTitle>

        <DialogContent
          sx={{
            display: "flex",
            flexDirection: "column",
            gap: 3,
            mt: 2,
            minWidth: 400
          }}
        >

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

        <DialogActions sx={{ px: 3, pb: 2 }}>
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