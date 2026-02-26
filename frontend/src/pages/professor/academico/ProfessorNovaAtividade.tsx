import {
  Typography,
  Box,
  Card,
  CardContent,
  TextField,
  Button,
  MenuItem,
  CircularProgress,
  Divider
} from "@mui/material";
import ArrowBackIcon from "@mui/icons-material/ArrowBack";
import SaveIcon from "@mui/icons-material/Save";
import AppLayout from "../../../components/layout/AppLayout";
import { useNavigate, useParams } from "react-router-dom";
import { useState } from "react";
import { api } from "../../../services/api";

export default function ProfessorNovaAtividade() {
  const { turmaDisciplinaId } = useParams();
  const navigate = useNavigate();
  const [loading, setLoading] = useState(false);

  const [form, setForm] = useState({
    titulo: "",
    descricao: "",
    dataEntrega: "",
    tipo: 1
  });

  function handleChange(
    e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>
  ) {
    setForm({
      ...form,
      [e.target.name]: e.target.value
    });
  }

  async function handleSubmit() {
    try {
      setLoading(true);

      await api.post("/Atividade", {
        ...form,
        turmaDisciplinaId
      });

      navigate(`/professor/turma/${turmaDisciplinaId}`);

    } catch (error) {
      console.error("Erro ao criar atividade:", error);
    } finally {
      setLoading(false);
    }
  }

  return (
    <AppLayout>

      {/* HEADER */}
      <Box mb={6} textAlign="center">
        <Typography variant="h4" fontWeight={600} gutterBottom>
          Criar Nova Atividade
        </Typography>

        <Typography variant="body1" color="text.secondary">
          Configure os dados da atividade para esta disciplina.
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

                {/* DADOS DA ATIVIDADE */}
                <Typography
                  fontWeight={600}
                  variant="h6"
                  textAlign="center"
                >
                  Dados da Atividade
                </Typography>

                <TextField
                  label="Título"
                  name="titulo"
                  value={form.titulo}
                  onChange={handleChange}
                  fullWidth
                />

                <TextField
                  label="Descrição"
                  name="descricao"
                  multiline
                  rows={4}
                  value={form.descricao}
                  onChange={handleChange}
                  fullWidth
                />

                <TextField
                  label="Data de Entrega"
                  name="dataEntrega"
                  type="datetime-local"
                  InputLabelProps={{ shrink: true }}
                  value={form.dataEntrega}
                  onChange={handleChange}
                  fullWidth
                />

                <TextField
                  select
                  label="Tipo"
                  name="tipo"
                  value={form.tipo}
                  onChange={(e) =>
                    setForm({
                      ...form,
                      tipo: Number(e.target.value)
                    })
                  }
                  fullWidth
                >
                  <MenuItem value={1}>Trabalho</MenuItem>
                  <MenuItem value={2}>Prova</MenuItem>
                  <MenuItem value={3}>Lista de Exercícios</MenuItem>
                </TextField>

                <Divider sx={{ my: 2 }} />

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
                    onClick={() =>
                      navigate(`/professor/turma/${turmaDisciplinaId}`)
                    }
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
                    sx={{
                      px: 5,
                      borderRadius: 3,
                      background: "linear-gradient(90deg, #1976d2, #26c6da)",
                      color: "#fff",
                      "&:hover": {
                        background:
                          "linear-gradient(90deg, #1565c0, #00acc1)"
                      }
                    }}
                  >
                    Criar Atividade
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