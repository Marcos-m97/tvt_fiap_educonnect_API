import {
  Typography,
  Box,
  Card,
  CardContent,
  TextField,
  Button,
  CircularProgress,
  Autocomplete
} from "@mui/material";
import ArrowBackIcon from "@mui/icons-material/ArrowBack";
import SaveIcon from "@mui/icons-material/Save";
import AppLayout from "../../../components/layout/AppLayout";
import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { api } from "../../../services/api";

interface Curso {
  id: number;
  nome: string;
}

export default function AdminDisciplinaForm() {
  const navigate = useNavigate();

  const [loading, setLoading] = useState(false);
  const [cursos, setCursos] = useState<Curso[]>([]);
  const [cursoSelecionado, setCursoSelecionado] = useState<Curso | null>(null);

  const [disciplina, setDisciplina] = useState({
    nome: "",
    descricao: "",
    cargaHoraria: 0
  });

  async function carregarCursos(search = "") {
    try {
      const response = await api.get("/curso", {
        params: {
          page: 1,
          pageSize: 10,
          search: search || undefined
        }
      });

      setCursos(response.data.data);
    } catch (error) {
      console.error("Erro ao carregar cursos:", error);
    }
  }

  async function handleSubmit() {
    if (!cursoSelecionado) return;

    try {
      setLoading(true);

      await api.post("/disciplina", {
        ...disciplina,
        cursoId: cursoSelecionado.id
      });

      navigate(-1);
    } catch (error) {
      console.error("Erro ao criar disciplina:", error);
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    carregarCursos();
  }, []);

  return (
    <AppLayout>

      {/* HEADER */}
      <Box mb={6} textAlign="center">
        <Typography variant="h4" fontWeight={600} gutterBottom>
          Criar Disciplina
        </Typography>

        <Typography variant="body1" color="text.secondary">
          Vincule a disciplina a um curso existente.
        </Typography>
      </Box>

      <Box maxWidth="1000px" mx="auto">

        <Card
          sx={{
            borderRadius: 4,
            boxShadow: 5,
            px: 6,
            py: 6
          }}
        >
          <CardContent sx={{ p: 0 }}>

            {loading && (
              <Box display="flex" justifyContent="center" py={4}>
                <CircularProgress />
              </Box>
            )}

            {!loading && (
              <Box display="flex" flexDirection="column" gap={4}>

                <TextField
                  label="Nome da Disciplina"
                  value={disciplina.nome}
                  onChange={(e) =>
                    setDisciplina({ ...disciplina, nome: e.target.value })
                  }
                  fullWidth
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
                />

                <Autocomplete
                  options={cursos}
                  getOptionLabel={(option) => option.nome}
                  value={cursoSelecionado}
                  onChange={(_, newValue) =>
                    setCursoSelecionado(newValue)
                  }
                  onInputChange={(_, newInputValue) => {
                    carregarCursos(newInputValue);
                  }}
                  renderInput={(params) => (
                    <TextField
                      {...params}
                      label="Curso"
                      placeholder="Pesquisar curso..."
                      fullWidth
                    />
                  )}
                />

                {/* BOTÕES CENTRALIZADOS */}
                <Box
                  display="flex"
                  justifyContent="center"
                  gap={3}
                  mt={3}
                >
                  <Button
                    variant="outlined"
                    startIcon={<ArrowBackIcon />}
                    onClick={() => navigate(-1)}
                    sx={{
                      px: 5,
                      borderRadius: 3
                    }}
                  >
                    Voltar
                  </Button>

                  <Button
                    startIcon={<SaveIcon />}
                    onClick={handleSubmit}
                    disabled={
                      !disciplina.nome ||
                      !disciplina.descricao ||
                      disciplina.cargaHoraria <= 0 ||
                      !cursoSelecionado
                    }
                    sx={{
                      px: 5,
                      borderRadius: 3,
                      background: "linear-gradient(90deg, #1976d2, #26c6da)",
                      color: "#fff",
                      "&:hover": {
                        background: "linear-gradient(90deg, #1565c0, #00acc1)"
                      }
                    }}
                  >
                    Criar Disciplina
                  </Button>
                </Box>

              </Box>
            )}

          </CardContent>
        </Card>

      </Box>

    </AppLayout>
  );
}