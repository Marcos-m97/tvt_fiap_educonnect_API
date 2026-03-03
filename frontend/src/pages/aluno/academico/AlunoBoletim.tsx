import {
  Typography,
  Box,
  Card,
  CardContent,
  Button,
  CircularProgress,
  Chip,
  Divider,
  Accordion,
  AccordionSummary,
  AccordionDetails,
  Avatar
} from "@mui/material";
import ExpandMoreIcon from "@mui/icons-material/ExpandMore";
import ArrowBackIcon from "@mui/icons-material/ArrowBack";
import PictureAsPdfIcon from "@mui/icons-material/PictureAsPdf";
import PersonIcon from "@mui/icons-material/Person";
import AppLayout from "../../../components/layout/AppLayout";
import { useNavigate } from "react-router-dom";
import { useEffect, useState } from "react";
import { api } from "../../../services/api";

interface Boletim {
  id: number;
  alunoId: number;
  turmaId: number;
  geradoEm: string;
  disciplinas: DisciplinaBoletim[];
}

interface DisciplinaBoletim {
  nomeDisciplina: string;
  totalAtividades: number;
  nota: number;
  media: number;
  situacao: string;
  atividades: AtividadeBoletim[];
}

interface AtividadeBoletim {
  atividadeId: number;
  titulo: string;
  nota: number | null;
  entregue: boolean;
}

interface UsuarioLogado {
  id: number;
  nome: string;
  email: string;
  tipo: number;
  fotoPerfilUrl?: string;
}

export default function AlunoBoletim() {
  const navigate = useNavigate();

  const [boletins, setBoletins] = useState<Boletim[]>([]);
  const [preview, setPreview] = useState<Boletim | null>(null);
  const [loading, setLoading] = useState(true);
  const [usuario, setUsuario] = useState<UsuarioLogado | null>(null);

  const baseUrl = api.defaults.baseURL?.replace("/api", "");

  async function carregarBoletins(alunoId: number) {
    const response = await api.get(`/boletins/aluno/${alunoId}`);
    return response.data;
  }

  async function carregarPreview(alunoId: number, turmaId: number) {
    const response = await api.get(
      `/boletins/preview/${alunoId}/${turmaId}`
    );
    return response.data;
  }

  function gerarPDF(boletimId: number) {
    window.open(
      `https://localhost:7286/api/boletins/${boletimId}/pdf`,
      "_blank"
    );
  }

  useEffect(() => {
    async function init() {
      try {
        // 🔥 Buscar usuário logado (já traz fotoPerfilUrl)
        const me = await api.get("/account/me");
        setUsuario(me.data.usuario);

        // 🔹 Buscar contexto do aluno logado
        const contexto = await api.get("/account/me/contexto");
        const alunoId = contexto.data.alunoId;
        const turmaId = contexto.data.turmaId;

        const lista = await carregarBoletins(alunoId);

        if (lista.length > 0) {
          setBoletins(lista);
        } else {
          const prev = await carregarPreview(alunoId, turmaId);
          setPreview(prev);
        }

      } catch (error) {
        console.error("Erro ao carregar boletim:", error);
      } finally {
        setLoading(false);
      }
    }

    init();
  }, []);

  if (loading) {
    return (
      <AppLayout>
        <Box display="flex" justifyContent="center" py={6}>
          <CircularProgress />
        </Box>
      </AppLayout>
    );
  }

  return (
    <AppLayout>

      {/* HEADER COM FOTO PADRÃO */}
      <Box
        mb={4}
        display="flex"
        justifyContent="space-between"
        alignItems="center"
      >
        <Box display="flex" alignItems="center" gap={3}>
          <Avatar
            src={
              usuario?.fotoPerfilUrl
                ? `${baseUrl}${usuario.fotoPerfilUrl}`
                : undefined
            }
            sx={{ width: 80, height: 80 }}
          >
            <PersonIcon sx={{ fontSize: 40 }} />
          </Avatar>

          <Box>
            <Typography variant="h4" fontWeight={700}>
              {usuario?.nome}
            </Typography>

            <Typography color="text.secondary">
              Meu Boletim
            </Typography>
          </Box>
        </Box>

        <Button
          variant="outlined"
          startIcon={<ArrowBackIcon />}
          onClick={() => navigate(-1)}
        >
          Voltar
        </Button>
      </Box>

      <Divider sx={{ mb: 4 }} />

      {/* PREVIEW */}
      {preview && boletins.length === 0 && (
        <Card sx={{ mb: 4 }}>
          <CardContent>
            <Typography variant="h6" mb={3}>
              Notas Parciais
            </Typography>

            {preview.disciplinas.map((disciplina, index) => (
              <Accordion key={index}>
                <AccordionSummary expandIcon={<ExpandMoreIcon />}>
                  <Box
                    display="flex"
                    justifyContent="space-between"
                    width="100%"
                    alignItems="center"
                  >
                    <Typography fontWeight={600}>
                      {disciplina.nomeDisciplina}
                    </Typography>

                    <Chip
                      label={`Média atual: ${disciplina.media.toFixed(1)}`}
                      color="primary"
                      size="small"
                    />
                  </Box>
                </AccordionSummary>

                <AccordionDetails>
                  {disciplina.atividades.map((atividade) => (
                    <Box
                      key={atividade.atividadeId}
                      display="flex"
                      justifyContent="space-between"
                      mb={1}
                    >
                      <Typography>
                        {atividade.titulo}
                      </Typography>

                      <Typography>
                        Nota: {atividade.nota ?? "-"}
                      </Typography>
                    </Box>
                  ))}
                </AccordionDetails>
              </Accordion>
            ))}
          </CardContent>
        </Card>
      )}

      {/* BOLETIM FINAL */}
      {boletins.map((boletim) => (
        <Card key={boletim.id} sx={{ mb: 4 }}>
          <CardContent>
            <Box
              display="flex"
              justifyContent="space-between"
              alignItems="center"
              mb={3}
            >
              <Box>
                <Typography fontWeight={600}>
                  Boletim Final
                </Typography>
                <Typography variant="body2" color="text.secondary">
                  Gerado em {new Date(boletim.geradoEm).toLocaleString()}
                </Typography>
              </Box>

              <Button
                variant="outlined"
                startIcon={<PictureAsPdfIcon />}
                onClick={() => gerarPDF(boletim.id)}
              >
                PDF
              </Button>
            </Box>

            {boletim.disciplinas.map((disciplina, index) => (
              <Accordion key={index}>
                <AccordionSummary expandIcon={<ExpandMoreIcon />}>
                  <Box
                    display="flex"
                    justifyContent="space-between"
                    width="100%"
                    alignItems="center"
                  >
                    <Typography fontWeight={600}>
                      {disciplina.nomeDisciplina}
                    </Typography>

                    <Box display="flex" gap={2}>
                      <Chip
                        label={`Média: ${disciplina.media}`}
                        color="primary"
                        size="small"
                      />
                      <Chip
                        label={disciplina.situacao}
                        color={
                          disciplina.situacao === "Aprovado"
                            ? "success"
                            : "error"
                        }
                        size="small"
                      />
                    </Box>
                  </Box>
                </AccordionSummary>

                <AccordionDetails>
                  {disciplina.atividades.map((atividade) => (
                    <Box
                      key={atividade.atividadeId}
                      display="flex"
                      justifyContent="space-between"
                      mb={1}
                    >
                      <Typography>
                        {atividade.titulo}
                      </Typography>

                      <Typography>
                        Nota: {atividade.nota}
                      </Typography>
                    </Box>
                  ))}
                </AccordionDetails>
              </Accordion>
            ))}
          </CardContent>
        </Card>
      ))}

    </AppLayout>
  );
}