import {
  Typography,
  Box,
  Button,
  Divider,
  CircularProgress,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField
} from "@mui/material";
import ArrowBackIcon from "@mui/icons-material/ArrowBack";
import EditIcon from "@mui/icons-material/Edit";
import DownloadIcon from "@mui/icons-material/Download";
import CheckIcon from "@mui/icons-material/Check";
import AppLayout from "../../../components/layout/AppLayout";
import { useNavigate, useParams } from "react-router-dom";
import { useEffect, useState } from "react";
import { api } from "../../../services/api";

interface Entrega {
  id: number;
  nomeAluno: string;
  nota?: number;
  feedbackProfessor?: string;
  dataEnvio: string;
  arquivo: string;
}

interface AtividadeDetalhe {
  id: number;
  titulo: string;
  descricao: string;
  dataEntrega?: string;
  tipo?: number;
}

export default function ProfessorGerenciarAtividade() {
  const { turmaDisciplinaId, atividadeId } = useParams();
  const navigate = useNavigate();

  const [loading, setLoading] = useState(true);
  const [entregas, setEntregas] = useState<Entrega[]>([]);
  const [atividade, setAtividade] = useState<AtividadeDetalhe | null>(null);

  const [selectedEntrega, setSelectedEntrega] = useState<Entrega | null>(null);
  const [modalOpen, setModalOpen] = useState(false);

  const [nota, setNota] = useState("");
  const [feedback, setFeedback] = useState("");

  // 🔹 Modal edição atividade
  const [editModalOpen, setEditModalOpen] = useState(false);
  const [editTitulo, setEditTitulo] = useState("");
  const [editDescricao, setEditDescricao] = useState("");
  const [editDataEntrega, setEditDataEntrega] = useState("");
  const [editTipo, setEditTipo] = useState<number>(0);

  async function carregarAtividade() {
    try {
      const response = await api.get(`/Atividade/${atividadeId}`);
      setAtividade(response.data);
    } catch (error) {
      console.error("Erro ao carregar atividade:", error);
    }
  }

  async function carregarEntregas() {
    try {
      setLoading(true);
      const response = await api.get(
        `/Entrega/atividade/${atividadeId}`
      );
      setEntregas(response.data);
    } catch (error) {
      console.error("Erro ao carregar entregas:", error);
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    carregarAtividade();
    carregarEntregas();
  }, [atividadeId]);

  // 🔹 Abrir modal edição
  function abrirEditarModal() {
    if (!atividade) return;

    setEditTitulo(atividade.titulo);
    setEditDescricao(atividade.descricao);
    setEditDataEntrega(
      atividade.dataEntrega
        ? atividade.dataEntrega.slice(0, 16)
        : ""
    );
    setEditTipo(atividade.tipo ?? 0);

    setEditModalOpen(true);
  }

  async function atualizarAtividade() {
    try {
      await api.put(`/Atividade/${atividadeId}`, {
        titulo: editTitulo,
        descricao: editDescricao,
        dataEntrega: editDataEntrega,
        tipo: editTipo
      });

      setEditModalOpen(false);
      carregarAtividade();

    } catch (error) {
      console.error("Erro ao atualizar atividade:", error);
    }
  }

  function abrirModal(entrega: Entrega) {
    setSelectedEntrega(entrega);
    setNota(entrega.nota?.toString() || "");
    setFeedback(entrega.feedbackProfessor || "");
    setModalOpen(true);
  }

  async function salvarCorrecao() {
    if (!selectedEntrega) return;

    try {
      await api.put(
        `/Entrega/${selectedEntrega.id}/corrigir`,
        {
          nota: Number(nota),
          feedback
        }
      );

      setModalOpen(false);
      carregarEntregas();

    } catch (error) {
      console.error("Erro ao corrigir:", error);
    }
  }

  function baixarArquivo(path: string) {
    window.open(`https://localhost:7286${path}`, "_blank");
  }

  return (
    <AppLayout>

      {/* HEADER */}
      <Box textAlign="center" mb={5}>
        <Typography variant="h3" fontWeight={700} letterSpacing={1}>
          Gerenciar Atividade
        </Typography>
      </Box>

      {/* BOTÕES SUPERIORES */}
      <Box display="flex" justifyContent="space-between" mb={4}>
        <Box display="flex" gap={2}>
          <Button
            startIcon={<EditIcon />}
            variant="outlined"
            sx={{ borderRadius: 3, px: 3 }}
            onClick={abrirEditarModal}
          >
            Editar
          </Button>
        </Box>

        <Button
          startIcon={<ArrowBackIcon />}
          variant="outlined"
          sx={{ borderRadius: 3, px: 3 }}
          onClick={() =>
            navigate(`/professor/turma/${turmaDisciplinaId}`)
          }
        >
          Voltar
        </Button>
      </Box>

      <Divider sx={{ mb: 4 }} />

      {/* ENUNCIADO */}
      {atividade && (
        <Box mb={5}>
          <Typography variant="h5" fontWeight={700}>
            {atividade.titulo}
          </Typography>

          {atividade.dataEntrega && (
            <Typography variant="body2" color="text.secondary" mt={1}>
              Entrega até: {atividade.dataEntrega}
            </Typography>
          )}

          <Typography
            variant="body1"
            mt={2}
            sx={{
              backgroundColor: "#f1f5f9",
              p: 3,
              borderRadius: 3
            }}
          >
            {atividade.descricao}
          </Typography>
        </Box>
      )}

      {/* LISTA ENTREGAS */}
      {loading && (
        <Box display="flex" justifyContent="center" py={4}>
          <CircularProgress />
        </Box>
      )}

      {!loading && entregas.length === 0 && (
        <Typography color="text.secondary">
          Nenhuma entrega realizada.
        </Typography>
      )}

      {!loading &&
        entregas.map((entrega) => (
          <Box
            key={entrega.id}
            sx={{
              py: 3,
              px: 3,
              borderRadius: 4,
              backgroundColor: "#f8fafc",
              display: "flex",
              justifyContent: "space-between",
              alignItems: "center",
              mb: 3
            }}
          >
            <Box>
              <Typography fontWeight={600} fontSize={18}>
                {entrega.nomeAluno}
              </Typography>

              <Typography variant="body2" color="text.secondary" mt={0.5}>
                Enviado em: {entrega.dataEnvio}
              </Typography>

              {entrega.nota !== undefined && (
                <Typography mt={1} fontWeight={600} color="primary">
                  Nota: {entrega.nota}
                </Typography>
              )}
            </Box>

            <Box display="flex" gap={2} alignItems="center">
              <Button
                startIcon={<DownloadIcon />}
                variant="outlined"
                sx={{ borderRadius: 3, px: 3 }}
                onClick={() => baixarArquivo(entrega.arquivo)}
              >
                Baixar
              </Button>

              <Button
                startIcon={<CheckIcon />}
                variant="contained"
                sx={{
                  borderRadius: 3,
                  px: 4,
                  background:
                    "linear-gradient(90deg, #1976d2, #26c6da)"
                }}
                onClick={() => abrirModal(entrega)}
              >
                Corrigir
              </Button>
            </Box>
          </Box>
        ))}

      {/* MODAL EDITAR ATIVIDADE */}
      <Dialog open={editModalOpen} onClose={() => setEditModalOpen(false)} fullWidth maxWidth="sm">
        <DialogTitle>Editar Atividade</DialogTitle>

        <DialogContent sx={{ display: "flex", flexDirection: "column", gap: 3, mt: 2 }}>
          <TextField
            label="Título"
            value={editTitulo}
            onChange={(e) => setEditTitulo(e.target.value)}
            fullWidth
          />

          <TextField
            label="Descrição"
            value={editDescricao}
            onChange={(e) => setEditDescricao(e.target.value)}
            multiline
            rows={4}
            fullWidth
          />

          <TextField
            label="Data de Entrega"
            type="datetime-local"
            value={editDataEntrega}
            onChange={(e) => setEditDataEntrega(e.target.value)}
            InputLabelProps={{ shrink: true }}
            fullWidth
          />
        </DialogContent>

        <DialogActions>
          <Button onClick={() => setEditModalOpen(false)}>
            Cancelar
          </Button>
          <Button variant="contained" onClick={atualizarAtividade}>
            Salvar Alterações
          </Button>
        </DialogActions>
      </Dialog>

      {/* MODAL CORREÇÃO */}
      <Dialog open={modalOpen} onClose={() => setModalOpen(false)} fullWidth maxWidth="sm">
        <DialogTitle>Corrigir Entrega</DialogTitle>

        <DialogContent sx={{ display: "flex", flexDirection: "column", gap: 3, mt: 2 }}>
          <TextField
            label="Nota"
            value={nota}
            onChange={(e) => setNota(e.target.value)}
            type="number"
            fullWidth
          />

          <TextField
            label="Feedback"
            multiline
            rows={4}
            value={feedback}
            onChange={(e) => setFeedback(e.target.value)}
            fullWidth
          />
        </DialogContent>

        <DialogActions>
          <Button onClick={() => setModalOpen(false)}>
            Cancelar
          </Button>

          <Button variant="contained" onClick={salvarCorrecao}>
            Salvar Correção
          </Button>
        </DialogActions>
      </Dialog>

    </AppLayout>
  );
}