import {
  Typography,
  Box,
  Card,
  CardContent,
  Button,
  Divider,
  CircularProgress
} from "@mui/material";
import ArrowBackIcon from "@mui/icons-material/ArrowBack";
import PlayCircleOutlineIcon from "@mui/icons-material/PlayCircleOutline";
import DownloadIcon from "@mui/icons-material/Download";
import AppLayout from "../../../components/layout/AppLayout";
import { useEffect, useState } from "react";
import { api } from "../../../services/api";
import { useNavigate, useParams } from "react-router-dom";

interface Aula {
  id: number;
  titulo: string;
  descricao: string;
  urlVideo?: string;
  materialApoio?: string | null;
  observacoes?: string;
  criadoEm: string;
}

export default function AlunoAulaDetalhe() {
  const { aulaId } = useParams();
  const navigate = useNavigate();

  const [loading, setLoading] = useState(true);
  const [aula, setAula] = useState<Aula | null>(null);

  useEffect(() => {
    async function carregarAula() {
      try {
        const response = await api.get<Aula>(`/aulas/${aulaId}`);
        setAula(response.data);
      } catch (error) {
        console.error("Erro ao carregar aula:", error);
      } finally {
        setLoading(false);
      }
    }

    carregarAula();
  }, [aulaId]);

  if (loading) {
    return (
      <AppLayout>
        <Box display="flex" justifyContent="center" mt={10}>
          <CircularProgress />
        </Box>
      </AppLayout>
    );
  }

  if (!aula) {
    return (
      <AppLayout>
        <Typography>Aula não encontrada.</Typography>
      </AppLayout>
    );
  }

  return (
    <AppLayout>
      <Box maxWidth="1000px" mx="auto">

        {/* HEADER */}
        <Box textAlign="center" mb={4}>
          <Typography variant="h4" fontWeight={800} sx={{ mb: 1 }}>
            {aula.titulo}
          </Typography>

          <Typography variant="body2" color="text.secondary">
            {new Date(aula.criadoEm).toLocaleDateString()}
          </Typography>
        </Box>

        {/* VOLTAR */}
        <Box display="flex" justifyContent="flex-end" mb={3}>
          <Button
            variant="outlined"
            startIcon={<ArrowBackIcon />}
            onClick={() => navigate(-1)}
            sx={{ textTransform: "none" }}
          >
            Voltar
          </Button>
        </Box>

        <Card
          sx={{
            borderRadius: 4,
            border: "1px solid",
            borderColor: "divider",
            p: 3
          }}
        >
          <CardContent sx={{ p: 0 }}>

            {/* DESCRIÇÃO */}
            <Box mb={4}>
              <Typography variant="h6" fontWeight={700} sx={{ mb: 1 }}>
                Conteúdo da Aula
              </Typography>

              <Typography
                variant="body1"
                color="text.secondary"
                sx={{ lineHeight: 1.7 }}
              >
                {aula.descricao}
              </Typography>
            </Box>

            {/* OBSERVAÇÕES */}
            {aula.observacoes && (
              <>
                <Divider sx={{ mb: 3 }} />
                <Box mb={4}>
                  <Typography variant="subtitle1" fontWeight={700} sx={{ mb: 1 }}>
                    Observações
                  </Typography>

                  <Typography
                    variant="body2"
                    color="text.secondary"
                    sx={{ lineHeight: 1.7 }}
                  >
                    {aula.observacoes}
                  </Typography>
                </Box>
              </>
            )}

            <Divider sx={{ mb: 3 }} />

            {/* AÇÕES */}
            <Box display="flex" gap={2} flexWrap="wrap">

              {aula.urlVideo && (
                <Button
                  variant="contained"
                  startIcon={<PlayCircleOutlineIcon />}
                  href={aula.urlVideo}
                  target="_blank"
                  sx={{ textTransform: "none" }}
                >
                  Assistir Aula
                </Button>
              )}

              {aula.materialApoio && (
                <Button
                  variant="outlined"
                  startIcon={<DownloadIcon />}
                  href={`https://localhost:7286${aula.materialApoio}`}
                  target="_blank"
                  sx={{ textTransform: "none" }}
                >
                  Baixar Material
                </Button>
              )}

            </Box>

          </CardContent>
        </Card>
      </Box>
    </AppLayout>
  );
}