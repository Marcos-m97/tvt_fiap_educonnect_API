import {
  Typography,
  Box,
  Card,
  CardContent,
  Button,
  CircularProgress,
  Stack,
  Divider
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
  videoAula?: string;
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

      <Box maxWidth="900px" mx="auto" display="flex" flexDirection="column" gap={3}>

        {/* HEADER */}
        <Card elevation={2} sx={{ borderRadius: 2 }}>
          <CardContent sx={{ py: 3 }}>

            <Typography
              variant="h4"
              fontWeight={700}
              textAlign="center"
              mb={2}
            >
              {aula.titulo}
            </Typography>

            <Box display="flex" justifyContent="center">
              <Button
                variant="outlined"
                startIcon={<ArrowBackIcon />}
                onClick={() => navigate(-1)}
                sx={{ textTransform: "none" }}
              >
                Voltar
              </Button>
            </Box>

          </CardContent>
        </Card>


        {/* CARD PRINCIPAL DA AULA */}
        <Card elevation={2} sx={{ borderRadius: 2 }}>
          <CardContent sx={{ p: 4 }}>

            {/* DESCRIÇÃO */}
            <Typography variant="h6" fontWeight={700} mb={2}>
              Conteúdo da Aula
            </Typography>

            <Divider sx={{ mb: 3 }} />

            <Typography
              color="text.secondary"
              sx={{
                lineHeight: 1.8,
                mb: 4
              }}
            >
              {aula.descricao}
            </Typography>


            {/* VIDEO */}
            {aula.videoAula && (
              <>
                <Typography variant="h6" fontWeight={700} mb={2}>
                  Vídeo da Aula
                </Typography>

                <video
                  controls
                  width="100%"
                  style={{
                    borderRadius: 8,
                    marginBottom: 30,
                    background: "#000"
                  }}
                >
                  <source
                    src={`https://localhost:7286${aula.videoAula}`}
                    type="video/mp4"
                  />
                </video>
              </>
            )}


            {/* OBSERVAÇÕES */}
            {aula.observacoes && (
              <>
                <Typography variant="h6" fontWeight={700} mb={2}>
                  Observações
                </Typography>

                <Divider sx={{ mb: 2 }} />

                <Typography
                  color="text.secondary"
                  sx={{
                    lineHeight: 1.7,
                    mb: 4
                  }}
                >
                  {aula.observacoes}
                </Typography>
              </>
            )}


            {/* MATERIAIS */}
            {(aula.urlVideo || aula.materialApoio) && (
              <>
                <Typography variant="h6" fontWeight={700} mb={2}>
                  Materiais da Aula
                </Typography>

                <Divider sx={{ mb: 3 }} />

                <Stack
                  direction="row"
                  spacing={2}
                  flexWrap="wrap"
                  justifyContent="center"
                >

                  {aula.urlVideo && (
                    <Button
                      variant="contained"
                      startIcon={<PlayCircleOutlineIcon />}
                      href={aula.urlVideo}
                      target="_blank"
                      sx={{
                        textTransform: "none",
                        px: 3
                      }}
                    >
                      Vídeo Complementar
                    </Button>
                  )}

                  {aula.materialApoio && (
                    <Button
                      variant="outlined"
                      startIcon={<DownloadIcon />}
                      href={`https://localhost:7286${aula.materialApoio}`}
                      target="_blank"
                      sx={{
                        textTransform: "none",
                        px: 3
                      }}
                    >
                      Baixar Material
                    </Button>
                  )}

                </Stack>
              </>
            )}

          </CardContent>
        </Card>

      </Box>

    </AppLayout>
  );
}