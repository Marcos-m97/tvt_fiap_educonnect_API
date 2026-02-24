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

      <Box mb={6} textAlign="center">
        <Typography variant="h4" fontWeight={600} gutterBottom>
          Gerenciar Disciplina
        </Typography>

        {disciplina && (
          <Box mt={2}>
            {disciplina.ativo ? (
              <Chip label="Ativa" color="success" />
            ) : (
              <Chip label="Inativa" color="error" />
            )}
          </Box>
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
              boxShadow: 5,
              px: 6,
              py: 6,
              opacity: disciplina.ativo ? 1 : 0.6 // 🔥 visual apagado
            }}
          >
            <CardContent sx={{ p: 0 }}>
              <Box display="flex" flexDirection="column" gap={4}>

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

                <Divider sx={{ my: 2 }} />

                <Box display="flex" justifyContent="center" gap={3}>

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
                        startIcon={<SaveIcon />}
                        onClick={handleSalvar}
                        sx={{
                          background:
                            "linear-gradient(90deg, #1976d2, #26c6da)",
                          color: "#fff",
                          "&:hover": {
                            background:
                              "linear-gradient(90deg, #1565c0, #00acc1)"
                          }
                        }}
                      >
                        Salvar Alterações
                      </Button>

                      <Button
                        startIcon={<DeleteOutlineIcon />}
                        onClick={handleDesativar}
                        sx={{
                          background: "#d32f2f",
                          color: "#fff",
                          "&:hover": { background: "#b71c1c" }
                        }}
                      >
                        Desativar
                      </Button>
                    </>
                  )}

                  {!disciplina.ativo && (
                    <Button
                      startIcon={<RestoreIcon />}
                      onClick={handleReativar}
                      sx={{
                        background: "#2e7d32",
                        color: "#fff",
                        "&:hover": { background: "#1b5e20" }
                      }}
                    >
                      Reativar
                    </Button>
                  )}

                </Box>
              </Box>
            </CardContent>
          </Card>
        )}

      </Box>
    </AppLayout>
  );
}