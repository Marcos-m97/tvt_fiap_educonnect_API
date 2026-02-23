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
import EditIcon from "@mui/icons-material/Edit";
import DeleteIcon from "@mui/icons-material/Delete";
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

export default function AdminCursoDetalhe() {
  const { id } = useParams();
  const navigate = useNavigate();

  const [curso, setCurso] = useState<Curso | null>(null);
  const [loading, setLoading] = useState(false);

  async function carregarCurso() {
    try {
      setLoading(true);
      const response = await api.get(`/curso/${id}`);
      setCurso(response.data);
    } catch (error) {
      console.error("Erro ao carregar curso:", error);
    } finally {
      setLoading(false);
    }
  }

  async function deletarCurso() {
    if (!confirm("Tem certeza que deseja excluir este curso?"))
      return;

    try {
      await api.delete(`/curso/${id}`);
      navigate("/admin/academico/cursos");
    } catch (error) {
      console.error("Erro ao deletar curso:", error);
    }
  }

  useEffect(() => {
    carregarCurso();
  }, []);

  return (
    <AppLayout>

      {/* BOTÃO VOLTAR */}
      <Box mb={3}>
        <Button
          variant="outlined"
          startIcon={<ArrowBackIcon />}
          onClick={() => navigate("/admin/academico/cursos")}
        >
          Voltar
        </Button>
      </Box>

      {loading && (
        <Box display="flex" justifyContent="center" py={5}>
          <CircularProgress />
        </Box>
      )}

      {!loading && curso && (
        <>
          {/* HEADER DO CURSO */}
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

          {/* AÇÕES */}
          <Box display="flex" gap={2} mb={4}>
            <Button
              variant="contained"
              startIcon={<EditIcon />}
              onClick={() =>
                navigate(`/admin/academico/cursos/${curso.id}/editar`)
              }
            >
              Editar
            </Button>

            <Button
              variant="outlined"
              color="error"
              startIcon={<DeleteIcon />}
              onClick={deletarCurso}
            >
              Excluir
            </Button>
          </Box>

          {/* ÁREA CONTEXTUAL (PRÓXIMO PASSO) */}
          <Card>
            <CardContent>
              <Typography variant="h6" gutterBottom>
                Turmas
              </Typography>

              <Typography variant="body2" color="text.secondary">
                Nenhuma turma cadastrada ainda.
              </Typography>
            </CardContent>
          </Card>

          <Box mt={3}>
            <Card>
              <CardContent>
                <Typography variant="h6" gutterBottom>
                  Disciplinas
                </Typography>

                <Typography variant="body2" color="text.secondary">
                  Nenhuma disciplina cadastrada ainda.
                </Typography>
              </CardContent>
            </Card>
          </Box>
        </>
      )}

    </AppLayout>
  );
}