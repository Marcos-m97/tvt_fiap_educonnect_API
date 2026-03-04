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

      {/* HEADER MODERNO */}
      <Box mb={5} textAlign="center">
        <Typography variant="h3" fontWeight={600}>
          Criar Curso
        </Typography>

        <Typography variant="body1" color="text.secondary" mt={1}>
          Cadastre um novo curso na instituição.
        </Typography>
      </Box>

      <Box maxWidth="900px" mx="auto">

        <Card
          sx={{
            borderRadius: 4,
            boxShadow: 4,
            px: 5,
            py: 5
          }}
        >

          {/* BOTÕES NO TOPO */}
          <Box display="flex" justifyContent="flex-end" gap={2} mb={4}>
            <Button
              variant="outlined"
              startIcon={<ArrowBackIcon />}
              onClick={() => navigate("/admin/academico/cursos")}
            >
              Voltar
            </Button>

            <Button
              variant="contained"
              startIcon={<SaveIcon />}
              onClick={handleSubmit}
              disabled={
                !curso.nome ||
                !curso.descricao ||
                curso.cargaHoraria <= 0 ||
                loading
              }
            >
              {loading ? "Criando..." : "Criar Curso"}
            </Button>
          </Box>

          <Divider sx={{ mb: 4 }} />

          <CardContent sx={{ p: 0 }}>

            {loading && (
              <Box display="flex" justifyContent="center" py={4}>
                <CircularProgress />
              </Box>
            )}

            {!loading && (
              <Box
                display="grid"
                gridTemplateColumns={{
                  xs: "1fr",
                  md: "1fr 1fr"
                }}
                gap={4}
              >

                <Box gridColumn="1 / -1">
                  <TextField
                    label="Nome do Curso"
                    value={curso.nome}
                    onChange={(e) =>
                      setCurso({ ...curso, nome: e.target.value })
                    }
                    fullWidth
                    required
                  />
                </Box>

                <Box gridColumn="1 / -1">
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
                </Box>

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

              </Box>
            )}

          </CardContent>

        </Card>

      </Box>

    </AppLayout>
  );
}