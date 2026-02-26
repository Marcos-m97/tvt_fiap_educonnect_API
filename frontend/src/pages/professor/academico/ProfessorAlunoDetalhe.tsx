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
  Button
} from "@mui/material";
import ExpandMoreIcon from "@mui/icons-material/ExpandMore";
import ArrowBackIcon from "@mui/icons-material/ArrowBack";
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

export default function ProfessorAlunoDetalhe() {
  const { turmaDisciplinaId, alunoId } = useParams();
  const location = useLocation();
  const navigate = useNavigate();

  const nomeAluno = location.state?.nome || "";

  const [preview, setPreview] = useState<DisciplinaBoletim | null>(null);
  const [loading, setLoading] = useState(true);

  async function carregarDados() {
    try {
      if (!alunoId || !turmaDisciplinaId) return;

      // 🔹 1 - Buscar dados da TurmaDisciplina
      const turmaDisciplinaResponse = await api.get(
        `/turmadisciplina/${turmaDisciplinaId}`
      );

      const disciplinaNome =
        turmaDisciplinaResponse.data.disciplinaNome;

      // 🔹 2 - Buscar matrícula ativa do aluno
      const matriculaResponse = await api.get(
        `/matricula/aluno/${alunoId}/ativa`
      );

      const turmaId = matriculaResponse.data.turmaId;

      // 🔹 3 - Buscar preview completo (todas disciplinas)
      const previewResponse = await api.get(
        `/boletins/preview/${alunoId}/${turmaId}`
      );

      const todasDisciplinas: DisciplinaBoletim[] =
        previewResponse.data.disciplinas;

      // 🔹 4 - Filtrar apenas disciplina atual
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

      {/* HEADER */}
      <Box
        mb={3}
        display="flex"
        justifyContent="space-between"
        alignItems="center"
      >
        <Typography variant="h4">
          {nomeAluno}
        </Typography>

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