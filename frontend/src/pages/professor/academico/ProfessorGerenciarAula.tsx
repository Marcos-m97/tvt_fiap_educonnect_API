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
  CardContent
} from "@mui/material";
import ArrowBackIcon from "@mui/icons-material/ArrowBack";
import EditIcon from "@mui/icons-material/Edit";
import UploadIcon from "@mui/icons-material/Upload";
import AppLayout from "../../../components/layout/AppLayout";
import { useNavigate, useParams } from "react-router-dom";
import { useEffect, useState } from "react";
import { api } from "../../../services/api";

interface AulaDetalhe {
  id: number;
  titulo: string;
  descricao: string;
  urlVideo?: string;
  materialApoio?: string;
  observacoes?: string;
  criadoEm: string;
}

export default function ProfessorGerenciarAula() {
  const { turmaDisciplinaId, aulaId } = useParams();
  const navigate = useNavigate();

  const [loading, setLoading] = useState(true);
  const [aula, setAula] = useState<AulaDetalhe | null>(null);
  const [arquivo, setArquivo] = useState<File | null>(null);
  const [uploading, setUploading] = useState(false);

  const [openModal, setOpenModal] = useState(false);
  const [editTitulo, setEditTitulo] = useState("");
  const [editDescricao, setEditDescricao] = useState("");
  const [editUrlVideo, setEditUrlVideo] = useState("");
  const [editObservacoes, setEditObservacoes] = useState("");

  async function carregarAula() {
    try {
      const response = await api.get(`/aulas/${aulaId}`);
      setAula(response.data);
    } catch (error) {
      console.error("Erro ao carregar aula:", error);
    } finally {
      setLoading(false);
    }
  }

  async function uploadMaterial() {
    if (!arquivo) return;

    const formData = new FormData();
    formData.append("arquivo", arquivo);

    try {
      setUploading(true);

      await api.post(`/aulas/${aulaId}/material`, formData, {
        headers: { "Content-Type": "multipart/form-data" }
      });

      setArquivo(null);
      carregarAula();

    } catch (error) {
      console.error("Erro ao enviar material:", error);
    } finally {
      setUploading(false);
    }
  }

  function abrirModal() {
    if (!aula) return;

    setEditTitulo(aula.titulo);
    setEditDescricao(aula.descricao);
    setEditUrlVideo(aula.urlVideo || "");
    setEditObservacoes(aula.observacoes || "");
    setOpenModal(true);
  }

  async function atualizarAula() {
    try {
      await api.put(`/aulas/${aulaId}`, {
        titulo: editTitulo,
        descricao: editDescricao,
        urlVideo: editUrlVideo,
        observacoes: editObservacoes
      });

      setOpenModal(false);
      carregarAula();

    } catch (error) {
      console.error("Erro ao atualizar aula:", error);
    }
  }

  useEffect(() => {
    carregarAula();
  }, [aulaId]);

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
              Gerenciar Aula
            </Typography>

            <Box display="flex" gap={2}>

              <Button
                startIcon={<EditIcon />}
                variant="outlined"
                onClick={abrirModal}
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

      {loading && (
        <Box display="flex" justifyContent="center" py={5}>
          <CircularProgress />
        </Box>
      )}

      {!loading && aula && (
        <>
          {/* CONTEÚDO DA AULA */}
          <Card sx={{ mb: 4 }}>
            <CardContent>

              <Typography variant="h5" fontWeight={600}>
                {aula.titulo}
              </Typography>

              <Typography mt={2}>
                {aula.descricao}
              </Typography>

              {aula.urlVideo && (
                <Typography mt={2}>
                  Vídeo:{" "}
                  <a
                    href={aula.urlVideo}
                    target="_blank"
                    rel="noreferrer"
                  >
                    {aula.urlVideo}
                  </a>
                </Typography>
              )}

              {aula.observacoes && (
                <Typography mt={2} color="text.secondary">
                  {aula.observacoes}
                </Typography>
              )}

              <Typography mt={2} variant="caption">
                Criado em:{" "}
                {new Date(aula.criadoEm).toLocaleDateString()}
              </Typography>

            </CardContent>
          </Card>


          {/* MATERIAL DE APOIO */}
          <Card>
            <CardContent>

              <Typography variant="h6" fontWeight={600} mb={2}>
                Material de Apoio
              </Typography>

              {!aula.materialApoio && (
                <Typography color="text.secondary" mb={2}>
                  Nenhum material enviado.
                </Typography>
              )}

              {aula.materialApoio && (
                <Box mb={3}>
                  <Button
                    variant="outlined"
                    onClick={() =>
                      window.open(
                        `https://localhost:7286${aula.materialApoio}`,
                        "_blank"
                      )
                    }
                  >
                    Baixar Material
                  </Button>
                </Box>
              )}

              <Divider sx={{ my: 3 }} />

              <Box display="flex" alignItems="center" gap={2} flexWrap="wrap">

                <input
                  type="file"
                  onChange={(e) => {
                    if (e.target.files) {
                      setArquivo(e.target.files[0]);
                    }
                  }}
                />

                <Button
                  startIcon={<UploadIcon />}
                  variant="contained"
                  disabled={!arquivo || uploading}
                  onClick={uploadMaterial}
                >
                  {uploading ? "Enviando..." : "Enviar"}
                </Button>

              </Box>

            </CardContent>
          </Card>
        </>
      )}

      {/* MODAL EDITAR AULA */}
      <Dialog open={openModal} onClose={() => setOpenModal(false)} fullWidth maxWidth="sm">

        <DialogTitle>Editar Aula</DialogTitle>

        <DialogContent sx={{ mt: 1 }}>

          <TextField
            label="Título"
            fullWidth
            margin="normal"
            value={editTitulo}
            onChange={(e) => setEditTitulo(e.target.value)}
          />

          <TextField
            label="Descrição"
            fullWidth
            multiline
            rows={4}
            margin="normal"
            value={editDescricao}
            onChange={(e) => setEditDescricao(e.target.value)}
          />

          <TextField
            label="URL do Vídeo"
            fullWidth
            margin="normal"
            value={editUrlVideo}
            onChange={(e) => setEditUrlVideo(e.target.value)}
          />

          <TextField
            label="Observações"
            fullWidth
            multiline
            rows={3}
            margin="normal"
            value={editObservacoes}
            onChange={(e) => setEditObservacoes(e.target.value)}
          />

        </DialogContent>

        <DialogActions>

          <Button onClick={() => setOpenModal(false)}>
            Cancelar
          </Button>

          <Button
            variant="contained"
            onClick={atualizarAula}
          >
            Salvar Alterações
          </Button>

        </DialogActions>

      </Dialog>

    </AppLayout>
  );
}