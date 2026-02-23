import {
  Typography,
  Box,
  Card,
  CardContent,
  TextField,
  Button,
  CircularProgress
} from "@mui/material";
import ArrowBackIcon from "@mui/icons-material/ArrowBack";
import SaveIcon from "@mui/icons-material/Save";
import AppLayout from "../../../components/layout/AppLayout";
import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { api } from "../../../services/api";

export default function AdminCursoForm() {
  const navigate = useNavigate();
  const [loading, setLoading] = useState(false);

  const [curso, setCurso] = useState({
    nome: "",
    descricao: "",
    cargaHoraria: 0
  });

  async function handleSubmit() {
    try {
      setLoading(true);

      await api.post("/curso", curso);

      navigate("/admin/academico/cursos");

    } catch (error) {
      console.error("Erro ao criar curso:", error);
    } finally {
      setLoading(false);
    }
  }

  return (
    <AppLayout>

      {/* HEADER */}
      <Box mb={4}>
        <Typography
          variant="h3"
          fontWeight={550}
          textAlign="center"
          gutterBottom
        >
          Criar Curso
        </Typography>

        <Typography
          variant="body1"
          color="text.secondary"
          textAlign="center"
        >
          Cadastre um novo curso na instituição.
        </Typography>
      </Box>

      <Box maxWidth="900px" mx="auto">

        {/* BOTÃO VOLTAR */}
        <Box mb={2}>
          <Button
            variant="outlined"
            startIcon={<ArrowBackIcon />}
            onClick={() => navigate("/admin/academico/cursos")}
          >
            Voltar
          </Button>
        </Box>

        <Card
          sx={{
            borderRadius: 3,
            boxShadow: 3
          }}
        >
          <CardContent>

            {loading && (
              <Box display="flex" justifyContent="center" py={3}>
                <CircularProgress />
              </Box>
            )}

            {!loading && (
              <Box display="flex" flexDirection="column" gap={3}>

                <TextField
                  label="Nome do Curso"
                  value={curso.nome}
                  onChange={(e) =>
                    setCurso({ ...curso, nome: e.target.value })
                  }
                  fullWidth
                  required
                />

                <TextField
                  label="Descrição"
                  multiline
                  rows={4}
                  value={curso.descricao}
                  onChange={(e) =>
                    setCurso({ ...curso, descricao: e.target.value })
                  }
                  fullWidth
                  required
                />

                <TextField
                  label="Carga Horária"
                  type="number"
                  value={curso.cargaHoraria}
                  onChange={(e) =>
                    setCurso({
                      ...curso,
                      cargaHoraria: Number(e.target.value)
                    })
                  }
                  fullWidth
                  required
                />

                <Box display="flex" justifyContent="flex-end">
                  <Button
                    variant="contained"
                    startIcon={<SaveIcon />}
                    onClick={handleSubmit}
                    disabled={
                      !curso.nome ||
                      !curso.descricao ||
                      curso.cargaHoraria <= 0
                    }
                  >
                    Criar
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