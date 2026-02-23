import {
  Typography,
  Box,
  Card,
  CardContent,
  CardActions,
  Button,
  Divider,
  TextField,
  Pagination,
  CircularProgress
} from "@mui/material";
import AddIcon from "@mui/icons-material/Add";
import ArrowBackIcon from "@mui/icons-material/ArrowBack";
import AppLayout from "../../../components/layout/AppLayout";
import { useEffect, useState } from "react";
import { api } from "../../../services/api";
import { useNavigate } from "react-router-dom";

interface Curso {
  id: string;
  nome: string;
  descricao: string;
  cargaHoraria: number;
}

export default function AdminCursos() {
  const navigate = useNavigate();

  const [cursos, setCursos] = useState<Curso[]>([]);
  const [loading, setLoading] = useState(false);

  const [page, setPage] = useState(1);
  const [total, setTotal] = useState(0);
  const pageSize = 5;

  const [search, setSearch] = useState("");

  async function carregarCursos() {
    try {
      setLoading(true);

      const response = await api.get("/curso", {
        params: {
          page,
          pageSize,
          search: search || undefined
        }
      });

      setCursos(response.data.data);
      setTotal(response.data.total);

    } catch (error) {
      console.error("Erro ao carregar cursos:", error);
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    carregarCursos();
  }, [page, search]);

  const totalPages = Math.ceil(total / pageSize);

  return (
    <AppLayout>

      {/* HEADER CENTRALIZADO */}
      <Box textAlign="center" mb={4}>
        <Typography variant="h4" gutterBottom>
          Gestão de Cursos
        </Typography>

        <Typography variant="body1" color="text.secondary">
          Crie e gerencie os cursos da instituição.
        </Typography>
      </Box>

      {/* AÇÕES */}
      <Box
        display="flex"
        justifyContent="center"
        alignItems="center"
        gap={3}
        mb={4}
        flexWrap="wrap"
      >
        <Button
          variant="contained"
          startIcon={<AddIcon />}
          onClick={() => navigate("/admin/academico/cursos/novo")}
        >
          Criar Curso
        </Button>

        <TextField
          placeholder="Pesquisar por nome do curso"
          value={search}
          onChange={(e) => {
            setPage(1);
            setSearch(e.target.value);
          }}
          sx={{
            width: {
              xs: "100%",
              sm: 350,
              md: 450
            }
          }}
        />

        <Button
          variant="outlined"
          startIcon={<ArrowBackIcon />}
          onClick={() => navigate("/admin/academico")}
          sx={{ textTransform: "none" }}
        >
          Voltar
        </Button>
      </Box>

      {/* LISTA */}
      <Card
        sx={{
          transition: "0.2s",
          "&:hover": {
            boxShadow: 6
          }
        }}
      >
        <CardContent>

          {loading && (
            <Box display="flex" justifyContent="center" py={3}>
              <CircularProgress size={24} />
            </Box>
          )}

          {!loading && cursos.length === 0 && (
            <Typography variant="body2" color="text.secondary">
              Nenhum curso encontrado.
            </Typography>
          )}

          {!loading &&
            cursos.map((curso, index) => (
              <Box key={curso.id}>
                <Box
                  display="flex"
                  justifyContent="space-between"
                  alignItems="center"
                  py={2}
                >
                  <Box>
                    <Typography fontWeight={600}>
                      {curso.nome}
                    </Typography>

                    <Typography variant="body2" color="text.secondary">
                      {curso.descricao}
                    </Typography>

                    <Typography mt={1} variant="caption">
                      Carga Horária: {curso.cargaHoraria}h
                    </Typography>
                  </Box>

                  <Button
                    size="small"
                    variant="outlined"
                    onClick={() =>
                      navigate(`/admin/academico/cursos/${curso.id}`)
                    }
                    sx={{ textTransform: "none" }}
                  >
                    Detalhes
                  </Button>
                </Box>

                {index !== cursos.length - 1 && <Divider />}
              </Box>
            ))}

        </CardContent>

        <CardActions sx={{ justifyContent: "center", py: 2 }}>
          {totalPages > 1 && (
            <Pagination
              count={totalPages}
              page={page}
              onChange={(_, value) => setPage(value)}
              color="primary"
            />
          )}
        </CardActions>
      </Card>

    </AppLayout>
  );
}