import {
  Typography,
  Box,
  Card,
  CardContent,
  CircularProgress,
  Chip,
  Divider,
  Accordion,
  AccordionSummary,
  AccordionDetails,
  Button,
  Avatar
} from "@mui/material";
import ExpandMoreIcon from "@mui/icons-material/ExpandMore";
import ArrowBackIcon from "@mui/icons-material/ArrowBack";
import PersonIcon from "@mui/icons-material/Person";
import AppLayout from "../../../components/layout/AppLayout";
import { useNavigate, useParams, useLocation } from "react-router-dom";
import { useEffect, useState } from "react";
import { api } from "../../../services/api";

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

export default function ProfessorAlunoDetalhe() {
  const { turmaDisciplinaId, alunoId } = useParams();
  const location = useLocation();
  const navigate = useNavigate();

  const nomeAluno = location.state?.nome || "";

  const [preview, setPreview] = useState<DisciplinaBoletim | null>(null);
  const [loading, setLoading] = useState(true);
  const [matriculaAtiva, setMatriculaAtiva] = useState<MatriculaAtiva | null>(null);

  const baseUrl = api.defaults.baseURL?.replace("/api", "");

  async function carregarDados() {
    try {
      if (!alunoId || !turmaDisciplinaId) return;

      // 🔹 Buscar matrícula ativa (para pegar usuarioId)
      const matriculaResponse = await api.get(
        `/matricula/aluno/${alunoId}/ativa`
      );

      setMatriculaAtiva(matriculaResponse.data);

      const turmaId = matriculaResponse.data.turmaId;

      // 🔹 Buscar dados da TurmaDisciplina
      const turmaDisciplinaResponse = await api.get(
        `/turmadisciplina/${turmaDisciplinaId}`
      );

      const disciplinaNome =
        turmaDisciplinaResponse.data.disciplinaNome;

      // 🔹 Buscar preview completo
      const previewResponse = await api.get(
        `/boletins/preview/${alunoId}/${turmaId}`
      );

      const todasDisciplinas: DisciplinaBoletim[] =
        previewResponse.data.disciplinas;

      const disciplinaFiltrada = todasDisciplinas.find(
        (d) => d.nomeDisciplina === disciplinaNome
      );

      setPreview(disciplinaFiltrada || null);

    } catch (error) {
      console.error("Erro ao carregar dados do aluno:", error);
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    carregarDados();
  }, [alunoId, turmaDisciplinaId]);

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

      {/* HEADER COM FOTO IGUAL ADMIN */}
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
              {matriculaAtiva?.alunoNome || nomeAluno}
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

      {!preview && (
        <Typography color="text.secondary">
          Nenhuma informação encontrada para esta disciplina.
        </Typography>
      )}

      {preview && (
        <Card>
          <CardContent>

            <Typography variant="h6" mb={3}>
              Desempenho na Disciplina
            </Typography>

            <Box mb={3} display="flex" gap={2}>
              <Chip
                label={`Média: ${preview.media.toFixed(1)}`}
                color="primary"
              />

              <Chip
                label={preview.situacao}
                color={
                  preview.situacao === "Aprovado"
                    ? "success"
                    : "error"
                }
              />
            </Box>

            <Accordion defaultExpanded>
              <AccordionSummary expandIcon={<ExpandMoreIcon />}>
                <Typography fontWeight={600}>
                  Atividades
                </Typography>
              </AccordionSummary>

              <AccordionDetails>
                {preview.atividades.map((atividade) => (
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

          </CardContent>
        </Card>
      )}

    </AppLayout>
  );
}