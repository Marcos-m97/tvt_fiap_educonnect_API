import {
  Typography,
  Box,
  Card,
  CardContent,
  Button,
  CircularProgress,
  Chip
} from "@mui/material";
import ArrowBackIcon from "@mui/icons-material/ArrowBack";
import LaunchIcon from "@mui/icons-material/Launch";
import UploadFileIcon from "@mui/icons-material/UploadFile";
import AppLayout from "../../../components/layout/AppLayout";
import { useEffect, useState } from "react";
import { api } from "../../../services/api";
import { useNavigate, useParams } from "react-router-dom";

interface AtividadeDetalhe {
  id: number;
  titulo: string;
  descricao: string;
  dataEntrega: string;
  disciplinaNome: string;
  urlMaterial?: string;
}

interface Entrega {
  entregaId: number;
  atividadeId: number;
  dataEnvio: string;
  nota: number | null;
  feedbackProfessor: string | null;
  arquivo: string;
}

export default function AlunoAtividadeDetalhe() {
  const { atividadeId } = useParams();
  const navigate = useNavigate();

  const [loading, setLoading] = useState(true);
  const [atividade, setAtividade] = useState<AtividadeDetalhe | null>(null);
  const [entrega, setEntrega] = useState<Entrega | null>(null);
  const [arquivo, setArquivo] = useState<File | null>(null);
  const [enviando, setEnviando] = useState(false);

  function formatarData(data: string) {
    const d = new Date(data);

    const dia = String(d.getDate()).padStart(2, "0");
    const mes = String(d.getMonth() + 1).padStart(2, "0");
    const ano = d.getFullYear();

    const hora = String(d.getHours()).padStart(2, "0");
    const minuto = String(d.getMinutes()).padStart(2, "0");

    return `${dia}/${mes}/${ano} ${hora}:${minuto}`;
  }

  useEffect(() => {
    async function carregarDados() {
      try {
        if (!atividadeId) return;

        const atividadeRes = await api.get<AtividadeDetalhe>(
          `/atividade/${atividadeId}`
        );

        setAtividade(atividadeRes.data);

        const entregaRes = await api.get<Entrega[]>(
          `/entrega/minhas?atividadeId=${atividadeId}`
        );

        if (entregaRes.data.length > 0) {
          setEntrega(entregaRes.data[0]);
        }
      } catch (error) {
        console.error("Erro ao carregar atividade:", error);
      } finally {
        setLoading(false);
      }
    }

    carregarDados();
  }, [atividadeId]);

  async function enviarEntrega() {
    if (!arquivo || !atividadeId) return;

    try {
      setEnviando(true);

      const formData = new FormData();
      formData.append("arquivo", arquivo);

      await api.post(`/entrega/${atividadeId}`, formData, {
        headers: { "Content-Type": "multipart/form-data" }
      });

      alert("Entrega enviada com sucesso!");
      window.location.reload();
    } catch (error) {
      console.error("Erro ao enviar entrega:", error);
      alert("Erro ao enviar arquivo.");
    } finally {
      setEnviando(false);
    }
  }

  if (loading) {
    return (
      <AppLayout>
        <Box display="flex" justifyContent="center" mt={10}>
          <CircularProgress />
        </Box>
      </AppLayout>
    );
  }

  if (!atividade) {
    return (
      <AppLayout>
        <Typography>Atividade não encontrada.</Typography>
      </AppLayout>
    );
  }

  const entregue = !!entrega;

  return (
    <AppLayout>

      <Box maxWidth="900px" mx="auto">

        {/* HEADER */}
        <Box mb={4}>
          <Typography variant="h4" fontWeight={700}>
            {atividade.titulo}
          </Typography>

          <Typography color="text.secondary">
            {atividade.disciplinaNome}
          </Typography>
        </Box>

        {/* VOLTAR */}
        <Box mb={3}>
          <Button
            startIcon={<ArrowBackIcon />}
            variant="outlined"
            onClick={() => navigate(-1)}
          >
            Voltar
          </Button>
        </Box>

        {/* INFORMAÇÕES DA ATIVIDADE */}
        <Card
          sx={{
            borderRadius: 2,
            border: "1px solid",
            borderColor: "divider",
            mb: 3
          }}
        >
          <CardContent>

            <Box
              display="flex"
              justifyContent="space-between"
              alignItems="center"
              flexWrap="wrap"
            >

              <Box display="flex" gap={2} alignItems="center">

                <Chip
                  label={entregue ? "ENTREGUE" : "PENDENTE"}
                  color={entregue ? "success" : "warning"}
                  size="small"
                />

                <Typography fontWeight={500}>
                  Entrega até: {formatarData(atividade.dataEntrega)}
                </Typography>

              </Box>

              {atividade.urlMaterial && (
                <Button
                  startIcon={<LaunchIcon />}
                  href={atividade.urlMaterial}
                  target="_blank"
                  variant="contained"
                >
                  Abrir material
                </Button>
              )}

            </Box>

          </CardContent>
        </Card>

        {/* ENUNCIADO */}
        <Card
          sx={{
            borderRadius: 2,
            border: "1px solid",
            borderColor: "divider",
            mb: 3
          }}
        >
          <CardContent>

            <Typography variant="h6" fontWeight={600} mb={2}>
              Enunciado
            </Typography>

            <Typography
              color="text.secondary"
              sx={{ lineHeight: 1.8 }}
            >
              {atividade.descricao}
            </Typography>

          </CardContent>
        </Card>

        {/* ENTREGA */}
        {entrega && (
          <Card
            sx={{
              borderRadius: 2,
              border: "1px solid",
              borderColor: "divider"
            }}
          >
            <CardContent>

              <Typography variant="h6" fontWeight={600} mb={2}>
                Sua entrega
              </Typography>

              <Typography mb={2}>
                <strong>Nota:</strong> {entrega.nota ?? "Ainda não corrigido"}
              </Typography>

              {entrega.feedbackProfessor && (
                <Box mb={2}>
                  <Typography fontWeight={600}>
                    Feedback do professor
                  </Typography>

                  <Typography color="text.secondary">
                    {entrega.feedbackProfessor}
                  </Typography>
                </Box>
              )}

              <Button
                variant="outlined"
                href={`https://localhost:7286${entrega.arquivo}`}
                target="_blank"
              >
                Baixar arquivo enviado
              </Button>

            </CardContent>
          </Card>
        )}

        {!entregue && (
          <Card
            sx={{
              borderRadius: 2,
              border: "1px solid",
              borderColor: "divider"
            }}
          >
            <CardContent>

              <Typography variant="h6" fontWeight={600} mb={2}>
                Enviar entrega
              </Typography>

              <Box display="flex" gap={2}>

                <Button
                  component="label"
                  variant="outlined"
                  startIcon={<UploadFileIcon />}
                >
                  Selecionar arquivo
                  <input
                    type="file"
                    hidden
                    onChange={(e) =>
                      setArquivo(e.target.files?.[0] || null)
                    }
                  />
                </Button>

                <Button
                  variant="contained"
                  disabled={!arquivo || enviando}
                  onClick={enviarEntrega}
                >
                  {enviando ? "Enviando..." : "Enviar"}
                </Button>

              </Box>

            </CardContent>
          </Card>
        )}

      </Box>

    </AppLayout>
  );
}