import {
  Typography,
  Box,
  Card,
  CardContent,
  TextField,
  Button,
  CircularProgress,
  Divider,
  Alert
} from "@mui/material";
import ArrowBackIcon from "@mui/icons-material/ArrowBack";
import SaveIcon from "@mui/icons-material/Save";
import AppLayout from "../../../components/layout/AppLayout";
import { useNavigate, useParams } from "react-router-dom";
import { useState } from "react";
import { api } from "../../../services/api";

export default function ProfessorNovaAula() {
  const { turmaDisciplinaId } = useParams();
  const navigate = useNavigate();
  const [loading, setLoading] = useState(false);

  const [form, setForm] = useState({
    titulo: "",
    descricao: "",
    urlVideo: "",
    observacoes: ""
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

      await api.post("/aulas", {
        turmaDisciplinaId,
        ...form
      });

      navigate(`/professor/turma/${turmaDisciplinaId}`);

    } catch (error) {
      console.error("Erro ao criar aula:", error);
    } finally {
      setLoading(false);
    }
  }

  return (
    <AppLayout>

      {/* HEADER */}
      <Box mb={6} textAlign="center">
        <Typography variant="h3" fontWeight={600} gutterBottom>
          Criar Nova Aula
        </Typography>

        <Typography variant="body1" color="text.secondary">
          Configure os dados da aula para esta disciplina.
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

                {/* DADOS DA AULA */}
                <Typography
                  fontWeight={600}
                  variant="h6"
                  textAlign="center"
                >
                  Dados da Aula
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
                  label="Vídeo complementar (opcional)"
                  name="urlVideo"
                  value={form.urlVideo}
                  onChange={handleChange}
                  fullWidth
                />

                <TextField
                  label="Observações"
                  name="observacoes"
                  multiline
                  rows={3}
                  value={form.observacoes}
                  onChange={handleChange}
                  fullWidth
                />

                {/* INSTRUÇÃO SOBRE UPLOAD */}
                <Alert severity="info">
                  Após criar a aula você poderá enviar o <b>vídeo da aula (MP4)</b> 
                  e o <b>material de apoio</b> na tela de gerenciamento da aula.
                </Alert>

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
                    Criar Aula
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