import {
  Typography,
  Box,
  Button,
  Divider,
  CircularProgress
} from "@mui/material";
import ArrowBackIcon from "@mui/icons-material/ArrowBack";
import EditIcon from "@mui/icons-material/Edit";
import DeleteIcon from "@mui/icons-material/Delete";
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
    formData.append("arquivo", arquivo); // IMPORTANTE: key correta

    try {
      setUploading(true);

      await api.post(`/aulas/${aulaId}/material`, formData, {
        headers: { "Content-Type": "multipart/form-data" }
      });

      setArquivo(null);
      carregarAula(); // Atualiza dados da aula após upload

    } catch (error) {
      console.error("Erro ao enviar material:", error);
    } finally {
      setUploading(false);
    }
  }

  async function excluirAula() {
    try {
      await api.delete(`/aulas/${aulaId}`);
      navigate(`/professor/turma/${turmaDisciplinaId}`);
    } catch (error) {
      console.error("Erro ao excluir aula:", error);
    }
  }

  useEffect(() => {
    carregarAula();
  }, [aulaId]);

  return (
    <AppLayout>

      {/* HEADER */}
      <Box textAlign="center" mb={5}>
        <Typography variant="h4" fontWeight={700}>
          Gerenciar Aula
        </Typography>
      </Box>

      {/* BOTÕES SUPERIORES */}
      <Box display="flex" justifyContent="space-between" mb={4}>
        <Box display="flex" gap={2}>
          <Button
            startIcon={<EditIcon />}
            variant="outlined"
          >
            Editar
          </Button>

          <Button
            startIcon={<DeleteIcon />}
            color="error"
            onClick={excluirAula}
          >
            Excluir
          </Button>
        </Box>

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

      <Divider sx={{ mb: 4 }} />

      {loading && (
        <Box display="flex" justifyContent="center">
          <CircularProgress />
        </Box>
      )}

      {!loading && aula && (
        <>
          {/* DETALHES */}
          <Box mb={5}>
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
          </Box>

          <Divider sx={{ mb: 4 }} />

          {/* MATERIAL DE APOIO */}
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

          {/* UPLOAD */}
          <Box mt={3} display="flex" alignItems="center" gap={2}>
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
        </>
      )}

    </AppLayout>
  );
}