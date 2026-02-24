import {
  Typography,
  Box,
  Card,
  CardContent,
  Button,
  CircularProgress,
  Divider,
  Chip
} from "@mui/material";
import ArrowBackIcon from "@mui/icons-material/ArrowBack";
import AddIcon from "@mui/icons-material/Add";
import AppLayout from "../../../components/layout/AppLayout";
import { useNavigate, useParams } from "react-router-dom";
import { useEffect, useState } from "react";
import { api } from "../../../services/api";

interface Curso {
  id: number;
  nome: string;
  descricao: string;
  cargaHoraria: number;
}

interface Turma {
  id: number;
  nome: string;
  periodo: string;
  semestre: string;
}

interface Disciplina {
  id: number;
  nome: string;
  cargaHoraria: number;
  ativo: boolean;
}

export default function AdminCursoDetalhe() {
  const { id } = useParams();
  const navigate = useNavigate();

  const [curso, setCurso] = useState<Curso | null>(null);
  const [turmas, setTurmas] = useState<Turma[]>([]);
  const [disciplinas, setDisciplinas] = useState<Disciplina[]>([]);
  const [loading, setLoading] = useState(false);

  async function carregarDados() {
    try {
      setLoading(true);

      const cursoResponse = await api.get(`/curso/${id}`);
      const turmasResponse = await api.get(`/turma/curso/${id}`);
      const disciplinasResponse = await api.get(`/disciplina/curso/${id}`);

      setCurso(cursoResponse.data);
      setTurmas(turmasResponse.data);
      setDisciplinas(disciplinasResponse.data);

    } catch (error) {
      console.error("Erro ao carregar dados do curso:", error);
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    carregarDados();
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

  if (!curso) {
    return (
      <AppLayout>
        <Typography>Curso não encontrado.</Typography>
      </AppLayout>
    );
  }

  return (
    <AppLayout>

      <Box mb={3}>
        <Button
          variant="outlined"
          startIcon={<ArrowBackIcon />}
          onClick={() => navigate("/admin/academico/cursos")}
        >
          Voltar
        </Button>
      </Box>

      <Box mb={4}>
        <Typography variant="h4" gutterBottom>
          {curso.nome}
        </Typography>

        <Typography variant="body1" color="text.secondary">
          {curso.descricao}
        </Typography>

        <Box mt={2}>
          <Chip
            label={`Carga Horária: ${curso.cargaHoraria}h`}
            color="primary"
          />
        </Box>
      </Box>

      <Divider sx={{ mb: 4 }} />

      {/* TURMAS */}
      <Card sx={{ mb: 4 }}>
        <CardContent>
          <Box
            display="flex"
            justifyContent="space-between"
            alignItems="center"
            mb={2}
          >
            <Typography variant="h6">
              Turmas
            </Typography>

            <Button
              size="small"
              startIcon={<AddIcon />}
              onClick={() =>
                navigate(`/admin/academico/turmas/novo?cursoId=${curso.id}`)
              }
            >
              Adicionar Turma
            </Button>
          </Box>

          {turmas.length === 0 && (
            <Typography variant="body2" color="text.secondary">
              Nenhuma turma cadastrada.
            </Typography>
          )}

          {turmas.map((turma) => (
            <Box
              key={turma.id}
              py={1.5}
              display="flex"
              justifyContent="space-between"
            >
              <Box>
                <Typography fontWeight={600}>
                  {turma.nome}
                </Typography>
                <Typography variant="body2" color="text.secondary">
                  {turma.semestre} • {turma.periodo}
                </Typography>
              </Box>

              <Button
                size="small"
                onClick={() =>
                  navigate(`/admin/academico/turmas/${turma.id}`)
                }
              >
                Gerenciar
              </Button>
            </Box>
          ))}
        </CardContent>
      </Card>

      {/* DISCIPLINAS */}
      <Card>
        <CardContent>
          <Box
            display="flex"
            justifyContent="space-between"
            alignItems="center"
            mb={2}
          >
            <Typography variant="h6">
              Disciplinas
            </Typography>

            <Button
              size="small"
              startIcon={<AddIcon />}
              onClick={() =>
                navigate(`/admin/academico/disciplinas/novo?cursoId=${curso.id}`)
              }
            >
              Adicionar Disciplina
            </Button>
          </Box>

          {disciplinas.length === 0 && (
            <Typography variant="body2" color="text.secondary">
              Nenhuma disciplina cadastrada.
            </Typography>
          )}

          {disciplinas.map((disciplina) => (
            <Box
              key={disciplina.id}
              py={1.5}
              display="flex"
              justifyContent="space-between"
              alignItems="center"
              sx={{
                opacity: disciplina.ativo ? 1 : 0.4,
                transition: "0.3s"
              }}
            >
              <Box>
                <Box display="flex" alignItems="center" gap={1}>
                  <Typography fontWeight={600}>
                    {disciplina.nome}
                  </Typography>

                  {!disciplina.ativo && (
                    <Chip
                      label="Inativa"
                      size="small"
                      color="error"
                    />
                  )}
                </Box>

                <Typography variant="body2" color="text.secondary">
                  Carga Horária: {disciplina.cargaHoraria}h
                </Typography>
              </Box>

              <Box display="flex" gap={1}>
                {disciplina.ativo ? (
                  <Button
                    size="small"
                    onClick={() =>
                      navigate(`/admin/academico/disciplinas/${disciplina.id}`)
                    }
                  >
                    Gerenciar
                  </Button>
                ) : (
                  <Button
                    size="small"
                    color="success"
                    onClick={async () => {
                      await api.put(`/disciplina/reativar/${disciplina.id}`);
                      carregarDados();
                    }}
                  >
                    Reativar
                  </Button>
                )}
              </Box>
            </Box>
          ))}

        </CardContent>
      </Card>

    </AppLayout>
  );
}