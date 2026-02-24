import {
  Typography,
  Box,
  Card,
  CardContent,
  TextField,
  Button,
  CircularProgress,
  Divider
} from "@mui/material";
import SaveIcon from "@mui/icons-material/Save";
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

export default function AdminCursoEditar() {
  const { id } = useParams();
  const navigate = useNavigate();

  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);

  const [nome, setNome] = useState("");
  const [descricao, setDescricao] = useState("");
  const [cargaHoraria, setCargaHoraria] = useState<number | "">("");

  async function carregarCurso() {
    try {
      const response = await api.get(`/curso/${id}`);
      const curso: Curso = response.data;

      setNome(curso.nome);
      setDescricao(curso.descricao);
      setCargaHoraria(curso.cargaHoraria);
    } catch (error) {
      console.error("Erro ao carregar curso:", error);
    } finally {
      setLoading(false);
    }
  }

  async function salvar() {
    try {
      setSaving(true);

      await api.put(`/curso/${id}`, {
        nome,
        descricao,
        cargaHoraria: Number(cargaHoraria)
      });

      navigate(`/admin/academico/cursos/${id}`);
    } catch (error) {
      console.error("Erro ao atualizar curso:", error);
    } finally {
      setSaving(false);
    }
  }

  useEffect(() => {
    carregarCurso();
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

      <Typography variant="h4" gutterBottom>
        Editar Curso
      </Typography>

      <Divider sx={{ mb: 4 }} />

      <Card>
        <CardContent>

          <Box
            display="flex"
            flexDirection="column"
            gap={3}
          >

            <TextField
              label="Nome do Curso"
              value={nome}
              onChange={(e) => setNome(e.target.value)}
              fullWidth
            />

            <TextField
              label="Descrição"
              multiline
              rows={4}
              value={descricao}
              onChange={(e) => setDescricao(e.target.value)}
              fullWidth
            />

            <TextField
              label="Carga Horária"
              type="number"
              value={cargaHoraria}
              onChange={(e) => setCargaHoraria(Number(e.target.value))}
              fullWidth
            />

          </Box>

          <Box
            mt={4}
            display="flex"
            justifyContent="center"
            gap={2}
          >
            <Button
              variant="outlined"
              onClick={() => navigate(-1)}
            >
              Cancelar
            </Button>

            <Button
              variant="contained"
              startIcon={<SaveIcon />}
              onClick={salvar}
              disabled={!nome || !descricao || !cargaHoraria || saving}
            >
              Salvar Alterações
            </Button>
          </Box>

        </CardContent>
      </Card>

    </AppLayout>
  );
}