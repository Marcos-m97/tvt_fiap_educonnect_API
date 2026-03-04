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
import ArrowBackIcon from "@mui/icons-material/ArrowBack";
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

  const [nome, setNome] = useState("");
  const [periodo, setPeriodo] = useState("");
  const [semestre, setSemestre] = useState("");
  const [cursoId, setCursoId] = useState<string>("");

  async function carregarTurma() {
    try {
      const response = await api.get(`/turma/${id}`);
      const turma: Turma = response.data;

      setNome(turma.nome);
      setPeriodo(turma.periodo);
      setSemestre(turma.semestre);
      setCursoId(turma.cursoId);
    } catch (error) {
      console.error("Erro ao carregar turma:", error);
    } finally {
      setLoading(false);
    }
  }

  async function salvar() {
    try {
      setSaving(true);

      await api.put(`/turma/${id}`, {
        nome,
        periodo,
        semestre,
        cursoId
      });

      navigate(`/admin/academico/turmas/${id}`);
    } catch (error) {
      console.error("Erro ao atualizar turma:", error);
    } finally {
      setSaving(false);
    }
  }

  useEffect(() => {
    carregarTurma();
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

      {/* HEADER MODERNO */}
      <Box mb={5} textAlign="center">
        <Typography variant="h3" fontWeight={700}>
          Editar Turma
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

          {/* BOTÕES NO TOPO DO CARD */}
          <Box display="flex" justifyContent="flex-end" gap={2} mb={4}>
            <Button
              variant="outlined"
              startIcon={<ArrowBackIcon />}
              onClick={() => navigate(-1)}
            >
              Cancelar
            </Button>

            <Button
              variant="contained"
              startIcon={<SaveIcon />}
              onClick={salvar}
              disabled={!nome || !periodo || !semestre || saving}
            >
              {saving ? "Salvando..." : "Salvar"}
            </Button>
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

              <Box gridColumn="1 / -1">
                <TextField
                  label="Nome da Turma"
                  value={nome}
                  onChange={(e) => setNome(e.target.value)}
                  fullWidth
                />
              </Box>

              <TextField
                label="Período"
                value={periodo}
                onChange={(e) => setPeriodo(e.target.value)}
                fullWidth
              />

              <TextField
                label="Semestre"
                value={semestre}
                onChange={(e) => setSemestre(e.target.value)}
                fullWidth
              />

            </Box>

          </CardContent>
        </Card>

      </Box>

    </AppLayout>
  );
}