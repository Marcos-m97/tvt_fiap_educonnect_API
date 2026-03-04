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
  TextField,
  Card,
  CardContent,
  MenuItem
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
  urlMaterial?: string;
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

  const [editModalOpen, setEditModalOpen] = useState(false);
  const [editTitulo, setEditTitulo] = useState("");
  const [editDescricao, setEditDescricao] = useState("");
  const [editDataEntrega, setEditDataEntrega] = useState("");
  const [editTipo, setEditTipo] = useState<number>(1);
  const [editUrlMaterial, setEditUrlMaterial] = useState("");

  function formatarData(data?: string) {
    if (!data) return "";

    const d = new Date(data);

    const dia = String(d.getDate()).padStart(2, "0");
    const mes = String(d.getMonth() + 1).padStart(2, "0");
    const ano = d.getFullYear();

    const hora = String(d.getHours()).padStart(2, "0");
    const minuto = String(d.getMinutes()).padStart(2, "0");

    return `${dia}/${mes}/${ano} ${hora}:${minuto}`;
  }

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
      const response = await api.get(`/Entrega/atividade/${atividadeId}`);
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

  function abrirEditarModal() {
    if (!atividade) return;

    setEditTitulo(atividade.titulo);
    setEditDescricao(atividade.descricao);

    setEditDataEntrega(
      atividade.dataEntrega
        ? atividade.dataEntrega.slice(0, 16)
        : ""
    );

    setEditTipo(atividade.tipo ?? 1);
    setEditUrlMaterial(atividade.urlMaterial ?? "");

    setEditModalOpen(true);
  }

  async function atualizarAtividade() {
    try {
      await api.put(`/Atividade/${atividadeId}`, {
        titulo: editTitulo,
        descricao: editDescricao,
        dataEntrega: editDataEntrega,
        urlMaterial: editUrlMaterial,
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

            <Typography variant="h4" fontWeight={700}>
              Gerenciar Atividade
            </Typography>

            <Box display="flex" gap={2}>
              <Button
                startIcon={<EditIcon />}
                variant="outlined"
                onClick={abrirEditarModal}
              >
                Editar
              </Button>

              <Button
                startIcon={<ArrowBackIcon />}
                variant="outlined"
                onClick={() =>
                  navigate(`/professor/turma/${turmaDisciplinaId}`)
                }
              >
                Voltar
              </Button>
            </Box>

          </Box>

        </CardContent>
      </Card>

      {/* ENUNCIADO */}
      {atividade && (
        <Card sx={{ mb: 4 }}>
          <CardContent>

            <Typography variant="h5" fontWeight={700}>
              {atividade.titulo}
            </Typography>

            {atividade.dataEntrega && (
              <Typography variant="body2" color="text.secondary" mt={1}>
                Entrega até: {formatarData(atividade.dataEntrega)}
              </Typography>
            )}

            <Typography
              variant="body1"
              mt={3}
              sx={{
                backgroundColor: "rgba(0,0,0,0.03)",
                p: 3,
                borderRadius: 2
              }}
            >
              {atividade.descricao}
            </Typography>

            {atividade.urlMaterial && (
              <Box mt={2}>
                <Button
                  variant="outlined"
                  href={atividade.urlMaterial}
                  target="_blank"
                  rel="noopener noreferrer"
                >
                  Abrir Enunciado / Material
                </Button>
              </Box>
            )}

          </CardContent>
        </Card>
      )}

      {/* LISTA DE ENTREGAS */}
      <Card>
        <CardContent>

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
            entregas.map((entrega, index) => (
              <Box key={entrega.id}>

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
                      {entrega.nomeAluno}
                    </Typography>

                    <Typography variant="body2" color="text.secondary">
                      Enviado em: {entrega.dataEnvio}
                    </Typography>

                    {entrega.nota !== undefined && (
                      <Typography mt={0.5} color="primary">
                        Nota: {entrega.nota}
                      </Typography>
                    )}
                  </Box>

                  <Box display="flex" gap={2}>

                    <Button
                      startIcon={<DownloadIcon />}
                      variant="outlined"
                      onClick={() => baixarArquivo(entrega.arquivo)}
                    >
                      Baixar
                    </Button>

                    <Button
                      startIcon={<CheckIcon />}
                      variant="contained"
                      onClick={() => abrirModal(entrega)}
                    >
                      Corrigir
                    </Button>

                  </Box>

                </Box>

                {index !== entregas.length - 1 && <Divider />}

              </Box>
            ))}

        </CardContent>
      </Card>

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
            label="URL do Material / Enunciado"
            value={editUrlMaterial}
            onChange={(e) => setEditUrlMaterial(e.target.value)}
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

          <TextField
            label="Tipo de Atividade"
            select
            value={editTipo}
            onChange={(e) => setEditTipo(Number(e.target.value))}
            fullWidth
          >
            <MenuItem value={1}>Exercício</MenuItem>
            <MenuItem value={2}>Trabalho</MenuItem>
            <MenuItem value={3}>Prova</MenuItem>
            <MenuItem value={4}>Seminário</MenuItem>
          </TextField>

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