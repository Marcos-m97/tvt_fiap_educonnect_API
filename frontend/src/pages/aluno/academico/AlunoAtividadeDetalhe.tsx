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

  useEffect(() => {
    async function carregarDados() {
      try {
        if (!atividadeId) return;

        // 🔹 Buscar detalhes da atividade
        const atividadeRes = await api.get<AtividadeDetalhe>(
          `/atividade/${atividadeId}`
        );

        setAtividade(atividadeRes.data);

        // 🔹 Buscar entrega do aluno para essa atividade
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
      <Box maxWidth="1000px" mx="auto">

        {/* HEADER */}
        <Box textAlign="center" mb={4}>
          <Typography variant="h4" fontWeight={800} sx={{ mb: 1 }}>
            {atividade.titulo}
          </Typography>

          <Typography variant="body1" color="text.secondary">
            {atividade.disciplinaNome}
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

            {/* STATUS RETANGULAR */}
            <Box
              sx={{
                display: "inline-block",
                px: 2,
                py: 0.8,
                fontSize: 13,
                fontWeight: 700,
                borderRadius: "6px",
                backgroundColor: entregue
                  ? "success.main"
                  : "warning.main",
                color: "white",
                mb: 3
              }}
            >
              {entregue ? "ENTREGUE" : "PENDENTE"}
            </Box>

            <Typography mb={3}>
              <strong>Data de entrega:</strong>{" "}
              {new Date(atividade.dataEntrega).toLocaleDateString()}
            </Typography>

            <Divider sx={{ mb: 3 }} />

            {/* ENUNCIADO */}
            <Box mb={4}>
              <Typography variant="h6" fontWeight={700} sx={{ mb: 1 }}>
                Enunciado
              </Typography>

              <Typography
                variant="body1"
                color="text.secondary"
                sx={{ lineHeight: 1.7 }}
              >
                {atividade.descricao}
              </Typography>
            </Box>

            {/* SE TIVER ENTREGA */}
            {entrega && (
              <>
                <Divider sx={{ mb: 3 }} />

                <Box mb={3}>
                  <Typography variant="h6" fontWeight={700}>
                    Nota: {entrega.nota ?? "Ainda não corrigido"}
                  </Typography>
                </Box>

                {entrega.feedbackProfessor && (
                  <Box mb={3}>
                    <Typography
                      variant="subtitle1"
                      fontWeight={700}
                      sx={{ mb: 1 }}
                    >
                      Feedback do Professor
                    </Typography>

                    <Typography
                      variant="body2"
                      color="text.secondary"
                      sx={{ lineHeight: 1.7 }}
                    >
                      {entrega.feedbackProfessor}
                    </Typography>
                  </Box>
                )}

                <Button
                  variant="outlined"
                  href={`https://localhost:7286${entrega.arquivo}`}
                  target="_blank"
                  sx={{ textTransform: "none" }}
                >
                  Baixar Arquivo Enviado
                </Button>
              </>
            )}

            {/* SE NÃO ENTREGUE */}
            {!entregue && (
              <Box display="flex" gap={2} mt={3}>
                <Button
                  component="label"
                  variant="outlined"
                  sx={{ textTransform: "none" }}
                >
                  Selecionar Arquivo
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
                  sx={{ textTransform: "none" }}
                >
                  {enviando ? "Enviando..." : "Enviar"}
                </Button>
              </Box>
            )}

          </CardContent>
        </Card>
      </Box>
    </AppLayout>
  );
}