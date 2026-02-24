import {
  Typography,
  Box,
  Card,
  CardContent,
  TextField,
  Button,
  CircularProgress,
  Divider,
  Chip
} from "@mui/material";
import SaveIcon from "@mui/icons-material/Save";
import AppLayout from "../../../components/layout/AppLayout";
import { useNavigate, useParams } from "react-router-dom";
import { useEffect, useState } from "react";
import { api } from "../../../services/api";

interface Turma {
  id: number;
  nome: string;
  periodo: string;
  semestre: string;
  cursoId: string;
  ativo: boolean;
}

export default function AdminTurmaEditar() {
  const { id } = useParams();
  const navigate = useNavigate();

  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [turma, setTurma] = useState<Turma | null>(null);

  async function carregarTurma() {
    try {
      setLoading(true);
      const response = await api.get(`/turma/${id}`);
      setTurma(response.data);
    } catch (error) {
      console.error("Erro ao carregar turma:", error);
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    carregarTurma();
  }, [id]);

  async function handleSalvar() {
    if (!turma) return;

    try {
      setSaving(true);

      await api.put(`/turma/${id}`, {
        nome: turma.nome,
        periodo: turma.periodo,
        semestre: turma.semestre,
        cursoId: turma.cursoId
      });

      navigate(-1);

    } catch (error) {
      console.error("Erro ao atualizar turma:", error);
    } finally {
      setSaving(false);
    }
  }

  return (
    <AppLayout>

      <Box mb={6} textAlign="center">
        <Typography variant="h4" fontWeight={600} gutterBottom>
          Gerenciar Turma
        </Typography>

        {turma && (
          <Box mt={2}>
            {turma.ativo ? (
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

        {!loading && turma && (
          <Card
            sx={{
              borderRadius: 4,
              boxShadow: 5,
              px: 6,
              py: 6,
              opacity: turma.ativo ? 1 : 0.6
            }}
          >
            <CardContent sx={{ p: 0 }}>
              <Box display="flex" flexDirection="column" gap={4}>

                <TextField
                  label="Nome da Turma"
                  value={turma.nome}
                  onChange={(e) =>
                    setTurma({ ...turma, nome: e.target.value })
                  }
                  fullWidth
                  disabled={!turma.ativo}
                />

                <TextField
                  label="Período"
                  value={turma.periodo}
                  onChange={(e) =>
                    setTurma({ ...turma, periodo: e.target.value })
                  }
                  fullWidth
                  disabled={!turma.ativo}
                />

                <TextField
                  label="Semestre"
                  value={turma.semestre}
                  onChange={(e) =>
                    setTurma({ ...turma, semestre: e.target.value })
                  }
                  fullWidth
                  disabled={!turma.ativo}
                />

                <Divider sx={{ my: 2 }} />

                <Box display="flex" justifyContent="center" gap={3}>

                  <Button
                    variant="outlined"
                    onClick={() => navigate(-1)}
                  >
                    Cancelar
                  </Button>

                  <Button
                    startIcon={<SaveIcon />}
                    onClick={handleSalvar}
                    disabled={!turma.ativo || saving}
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

                </Box>

              </Box>
            </CardContent>
          </Card>
        )}

      </Box>
    </AppLayout>
  );
}