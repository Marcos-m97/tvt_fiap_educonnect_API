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
import AutoFixHighIcon from "@mui/icons-material/AutoFixHigh";
import PersonIcon from "@mui/icons-material/Person";
import AppLayout from "../../../components/layout/AppLayout";
import { useNavigate, useParams } from "react-router-dom";
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

interface MatriculaAtiva {
  alunoId: number;
  usuarioId: number;
  alunoNome: string;
  turmaId: number;
  turmaNome: string;
}

export default function AdminAlunoDetalhe() {
  const { id } = useParams();
  const navigate = useNavigate();

  const [boletins, setBoletins] = useState<Boletim[]>([]);
  const [preview, setPreview] = useState<Boletim | null>(null);
  const [loading, setLoading] = useState(true);
  const [gerando, setGerando] = useState(false);

  const [matriculaAtiva, setMatriculaAtiva] = useState<MatriculaAtiva | null>(null);

  const baseUrl = api.defaults.baseURL?.replace("/api", "");

  async function carregarMatriculaAtiva() {
    try {
      if (!id) return;

      const response = await api.get(`/matricula/aluno/${id}/ativa`);
      setMatriculaAtiva(response.data);
    } catch (error) {
      console.error("Erro ao carregar matrícula ativa:", error);
    }
  }

  async function carregarBoletins() {
    try {
      if (!id) return;

      const response = await api.get(`/boletins/aluno/${id}`);
      setBoletins(response.data);
    } catch (error) {
      console.error("Erro ao carregar boletins:", error);
    }
  }

  async function carregarPreview() {
    try {
      if (!id) return;

      const matriculaResponse = await api.get(
        `/matricula/aluno/${id}/ativa`
      );

      const turmaId = matriculaResponse.data.turmaId;

      const response = await api.get(
        `/boletins/preview/${id}/${turmaId}`
      );

      setPreview(response.data);
    } catch (error) {
      console.error("Erro ao carregar preview:", error);
    }
  }

  async function gerarBoletim() {
    try {
      if (!id) return;

      setGerando(true);

      const matriculaResponse = await api.get(
        `/matricula/aluno/${id}/ativa`
      );

      const turmaId = matriculaResponse.data.turmaId;

      await api.post(`/boletins`, {
        alunoId: Number(id),
        turmaId: turmaId
      });

      await carregarBoletins();
      setPreview(null);

    } catch (error) {
      console.error("Erro ao gerar boletim:", error);
      alert("Aluno não possui matrícula ativa.");
    } finally {
      setGerando(false);
    }
  }

  function gerarPDF(boletimId: number) {
    window.open(
      `https://localhost:7286/api/boletins/${boletimId}/pdf`,
      "_blank"
    );
  }

  useEffect(() => {
    async function init() {
      await carregarMatriculaAtiva();
      await carregarBoletins();

      const lista = await api.get(`/boletins/aluno/${id}`);
      if (lista.data.length === 0) {
        await carregarPreview();
      }

      setLoading(false);
    }

    init();
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

  return (
    <AppLayout>

      {/* HEADER MELHORADO COM FOTO */}
      <Box
        mb={4}
        display="flex"
        justifyContent="space-between"
        alignItems="center"
      >
        <Box display="flex" alignItems="center" gap={3}>
          <Avatar
            src={
              matriculaAtiva?.usuarioId
                ? `${baseUrl}/api/usuario/${matriculaAtiva.usuarioId}/foto`
                : undefined
            }
            sx={{ width: 80, height: 80 }}
          >
            <PersonIcon sx={{ fontSize: 40 }} />
          </Avatar>

          <Box>
            <Typography variant="h4" fontWeight={700}>
              {matriculaAtiva?.alunoNome}
            </Typography>

            <Typography color="text.secondary">
              Turma: {matriculaAtiva?.turmaNome}
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

      {/* BOTÃO GERAR */}
      {boletins.length === 0 && (
        <Box mb={4}>
          <Button
            variant="contained"
            startIcon={<AutoFixHighIcon />}
            onClick={gerarBoletim}
            disabled={gerando}
          >
            {gerando ? "Gerando..." : "Gerar Boletim"}
          </Button>
        </Box>
      )}

      {/* BOLETIM GERADO */}
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
                  Boletim {matriculaAtiva?.alunoNome}
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