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
import EditIcon from "@mui/icons-material/Edit";
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
  ativo: boolean;
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

      {/* HEADER DO CURSO */}
      <Card sx={{ mb: 4 }}>
        <CardContent>

          <Box
            display="flex"
            justifyContent="space-between"
            alignItems="flex-start"
            flexWrap="wrap"
            gap={2}
          >

            <Box maxWidth={700}>

              <Typography
                variant="h4"
                fontWeight={700}
                gutterBottom
              >
                {curso.nome}
              </Typography>

              <Typography
                variant="body1"
                color="text.secondary"
                sx={{ lineHeight: 1.7 }}
              >
                {curso.descricao}
              </Typography>

              <Box mt={2}>
                <Chip
                  label={`Carga Horária: ${curso.cargaHoraria}h`}
                  color="primary"
                  sx={{ fontWeight: 600 }}
                />
              </Box>

            </Box>

            <Box display="flex" gap={2}>

              <Button
                variant="outlined"
                startIcon={<EditIcon />}
                onClick={() =>
                  navigate(`/admin/academico/cursos/${curso.id}/editar`)
                }
              >
                Editar
              </Button>

              <Button
                variant="outlined"
                startIcon={<ArrowBackIcon />}
                onClick={() => navigate("/admin/academico/cursos")}
              >
                Voltar
              </Button>

            </Box>

          </Box>

        </CardContent>
      </Card>


      {/* TURMAS */}
      <Card sx={{ mb: 4 }}>
        <CardContent>

          <Box
            display="flex"
            justifyContent="space-between"
            alignItems="center"
            mb={3}
          >

            <Typography variant="h6" fontWeight={600}>
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

          {turmas.map((turma, index) => (
            <Box key={turma.id}>

              <Box
                py={2}
                display="flex"
                justifyContent="space-between"
                alignItems="center"
                sx={{
                  opacity: turma.ativo ? 1 : 0.4,
                  transition: "0.25s",
                  "&:hover": {
                    background: "rgba(0,0,0,0.03)",
                    borderRadius: 2,
                    px: 1
                  }
                }}
              >

                <Box>

                  <Box display="flex" alignItems="center" gap={1}>
                    <Typography fontWeight={600}>
                      {turma.nome}
                    </Typography>

                    {!turma.ativo && (
                      <Chip
                        label="Inativa"
                        size="small"
                        color="error"
                      />
                    )}
                  </Box>

                  <Typography variant="body2" color="text.secondary">
                    {turma.semestre} • {turma.periodo}
                  </Typography>

                </Box>

                <Box display="flex" gap={1}>

                  {turma.ativo ? (
                    <Button
                      size="small"
                      onClick={() =>
                        navigate(`/admin/academico/turmas/${turma.id}`)
                      }
                    >
                      Gerenciar
                    </Button>
                  ) : (
                    <Button
                      size="small"
                      color="success"
                      onClick={async () => {
                        await api.put(`/turma/reativar/${turma.id}`);
                        carregarDados();
                      }}
                    >
                      Reativar
                    </Button>
                  )}

                </Box>

              </Box>

              {index !== turmas.length - 1 && <Divider />}

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
            mb={3}
          >

            <Typography variant="h6" fontWeight={600}>
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

          {disciplinas.map((disciplina, index) => (
            <Box key={disciplina.id}>

              <Box
                py={2}
                display="flex"
                justifyContent="space-between"
                alignItems="center"
                sx={{
                  opacity: disciplina.ativo ? 1 : 0.4,
                  transition: "0.25s",
                  "&:hover": {
                    background: "rgba(0,0,0,0.03)",
                    borderRadius: 2,
                    px: 1
                  }
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

              {index !== disciplinas.length - 1 && <Divider />}

            </Box>
          ))}

        </CardContent>
      </Card>

    </AppLayout>
  );
}