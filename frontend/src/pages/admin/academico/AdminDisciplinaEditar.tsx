import {
  Typography,
  Box,
  Card,
  CardContent,
  TextField,
  Button,
  CircularProgress,
  Divider,
  Autocomplete,
  Chip
} from "@mui/material";
import ArrowBackIcon from "@mui/icons-material/ArrowBack";
import SaveIcon from "@mui/icons-material/Save";
import DeleteOutlineIcon from "@mui/icons-material/DeleteOutline";
import RestoreIcon from "@mui/icons-material/Restore";
import AppLayout from "../../../components/layout/AppLayout";
import { useParams, useNavigate } from "react-router-dom";
import { useEffect, useState } from "react";
import { api } from "../../../services/api";

interface Curso {
  id: number;
  nome: string;
}

export default function AdminDisciplinaEditar() {
  const { id } = useParams();
  const navigate = useNavigate();

  const [loading, setLoading] = useState(true);
  const [disciplina, setDisciplina] = useState<any>(null);
  const [cursos, setCursos] = useState<Curso[]>([]);
  const [cursoSelecionado, setCursoSelecionado] = useState<Curso | null>(null);

  async function carregarDados() {
    try {
      setLoading(true);

      const response = await api.get(`/disciplina/${id}`);
      setDisciplina(response.data);

      const cursosResponse = await api.get("/curso", {
        params: { page: 1, pageSize: 50 }
      });

      setCursos(cursosResponse.data.data);

      const cursoAtual = cursosResponse.data.data.find(
        (c: Curso) => c.id === response.data.cursoId
      );

      setCursoSelecionado(cursoAtual || null);

    } catch (error) {
      console.error("Erro ao carregar disciplina:", error);
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    carregarDados();
  }, [id]);

  async function handleSalvar() {
    try {
      setLoading(true);

      await api.put(`/disciplina/${id}`, {
        nome: disciplina.nome,
        descricao: disciplina.descricao,
        cargaHoraria: disciplina.cargaHoraria,
        cursoId: cursoSelecionado?.id
      });

      navigate(-1);

    } catch (error) {
      console.error("Erro ao atualizar disciplina:", error);
    } finally {
      setLoading(false);
    }
  }

  async function handleDesativar() {
    if (!window.confirm("Deseja realmente desativar esta disciplina?")) return;

    try {
      setLoading(true);
      await api.delete(`/disciplina/${id}`);
      await carregarDados();
    } catch (error) {
      console.error("Erro ao desativar disciplina:", error);
    } finally {
      setLoading(false);
    }
  }

  async function handleReativar() {
    try {
      setLoading(true);
      await api.put(`/disciplina/reativar/${id}`);
      await carregarDados();
    } catch (error) {
      console.error("Erro ao reativar disciplina:", error);
    } finally {
      setLoading(false);
    }
  }

  return (
    <AppLayout>

      {/* HEADER MODERNO */}
      <Box mb={5} textAlign="center">
        <Typography variant="h3" fontWeight={700} gutterBottom>
          Gerenciar Disciplina
        </Typography>

        {disciplina && (
          <Chip
            label={disciplina.ativo ? "Disciplina Ativa" : "Disciplina Inativa"}
            color={disciplina.ativo ? "success" : "error"}
            sx={{ mt: 1, fontWeight: 600 }}
          />
        )}
      </Box>

      <Box maxWidth="1000px" mx="auto">

        {loading && (
          <Box display="flex" justifyContent="center" py={6}>
            <CircularProgress />
          </Box>
        )}

        {!loading && disciplina && (
          <Card
            sx={{
              borderRadius: 4,
              boxShadow: 4,
              px: 5,
              py: 5,
              opacity: disciplina.ativo ? 1 : 0.7,
              transition: "all 0.3s ease"
            }}
          >

            {/* BOTÕES NO TOPO DO CARD */}
            <Box display="flex" justifyContent="flex-end" gap={2} mb={4}>
              <Button
                variant="outlined"
                startIcon={<ArrowBackIcon />}
                onClick={() => navigate(-1)}
              >
                Voltar
              </Button>

              {disciplina.ativo && (
                <>
                  <Button
                    variant="contained"
                    startIcon={<SaveIcon />}
                    onClick={handleSalvar}
                  >
                    Salvar
                  </Button>

                  <Button
                    variant="contained"
                    color="error"
                    startIcon={<DeleteOutlineIcon />}
                    onClick={handleDesativar}
                  >
                    Desativar
                  </Button>
                </>
              )}

              {!disciplina.ativo && (
                <Button
                  variant="contained"
                  color="success"
                  startIcon={<RestoreIcon />}
                  onClick={handleReativar}
                >
                  Reativar
                </Button>
              )}
            </Box>

            <Divider sx={{ mb: 4 }} />

            <CardContent sx={{ p: 0 }}>
              <Box
                display="grid"
                gridTemplateColumns={{
                  xs: "1fr",
                  md: "1fr 1fr"
                }}
                gap={4}
              >

                <TextField
                  label="Nome da Disciplina"
                  value={disciplina.nome}
                  onChange={(e) =>
                    setDisciplina({ ...disciplina, nome: e.target.value })
                  }
                  fullWidth
                  disabled={!disciplina.ativo}
                />

                <TextField
                  label="Carga Horária"
                  type="number"
                  value={disciplina.cargaHoraria}
                  onChange={(e) =>
                    setDisciplina({
                      ...disciplina,
                      cargaHoraria: Number(e.target.value)
                    })
                  }
                  fullWidth
                  disabled={!disciplina.ativo}
                />

                <Box gridColumn="1 / -1">
                  <TextField
                    label="Descrição"
                    multiline
                    rows={4}
                    value={disciplina.descricao}
                    onChange={(e) =>
                      setDisciplina({ ...disciplina, descricao: e.target.value })
                    }
                    fullWidth
                    disabled={!disciplina.ativo}
                  />
                </Box>

                <Box gridColumn="1 / -1">
                  <Autocomplete
                    options={cursos}
                    getOptionLabel={(option) => option.nome}
                    value={cursoSelecionado}
                    onChange={(_, newValue) => setCursoSelecionado(newValue)}
                    renderInput={(params) => (
                      <TextField {...params} label="Curso" fullWidth />
                    )}
                    disabled={!disciplina.ativo}
                  />
                </Box>

              </Box>
            </CardContent>

          </Card>
        )}

      </Box>
    </AppLayout>
  );
}