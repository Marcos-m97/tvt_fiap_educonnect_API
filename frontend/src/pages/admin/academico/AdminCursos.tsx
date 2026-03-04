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
  CircularProgress,
  Chip
} from "@mui/material";
import AddIcon from "@mui/icons-material/Add";
import ArrowBackIcon from "@mui/icons-material/ArrowBack";
import AppLayout from "../../../components/layout/AppLayout";
import { useEffect, useState } from "react";
import { api } from "../../../services/api";
import { useNavigate } from "react-router-dom";

interface Curso {
  id: number;
  nome: string;
  descricao: string;
  cargaHoraria: number;
  ativo: boolean;
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

  async function desativarCurso(id: number) {
    if (!confirm("Deseja desativar este curso?")) return;

    await api.delete(`/curso/${id}`);
    carregarCursos();
  }

  async function reativarCurso(id: number) {
    await api.put(`/curso/reativar/${id}`);
    carregarCursos();
  }

  useEffect(() => {
    carregarCursos();
  }, [page, search]);

  const totalPages = Math.ceil(total / pageSize);

  return (
    <AppLayout>

      {/* HEADER */}
      <Card sx={{ mb: 4 }}>
        <CardContent>

          <Box textAlign="center" maxWidth={650} mx="auto">

            <Typography variant="h4" fontWeight={700} gutterBottom>
              Gestão Acadêmica
            </Typography>

            <Typography
              variant="body1"
              color="text.secondary"
              sx={{ lineHeight: 1.7 }}
            >
              Busque um curso para gerenciar turmas, disciplinas e vínculos acadêmicos.
            </Typography>

          </Box>

          {/* AÇÕES */}
          <Box
            display="flex"
            justifyContent="center"
            alignItems="center"
            gap={2}
            mt={3}
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
              placeholder="Pesquisar curso..."
              value={search}
              size="small"
              onChange={(e) => {
                setPage(1);
                setSearch(e.target.value);
              }}
              sx={{
                width: {
                  xs: "100%",
                  sm: 320,
                  md: 420
                }
              }}
            />

            <Button
              variant="outlined"
              startIcon={<ArrowBackIcon />}
              onClick={() => navigate("/admin")}
            >
              Voltar
            </Button>

          </Box>

        </CardContent>
      </Card>


      {/* LISTA DE CURSOS */}
      <Card>
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
                  sx={{
                    opacity: curso.ativo ? 1 : 0.45,
                    transition: "0.25s",
                    "&:hover": {
                      background: "rgba(0,0,0,0.03)",
                      borderRadius: 2,
                      px: 1
                    }
                  }}
                >

                  <Box maxWidth={600}>

                    <Box display="flex" alignItems="center" gap={1}>

                      <Typography fontWeight={600}>
                        {curso.nome}
                      </Typography>

                      {!curso.ativo && (
                        <Chip
                          label="Inativo"
                          size="small"
                          color="error"
                        />
                      )}

                      <Chip
                        label={`${curso.cargaHoraria}h`}
                        size="small"
                        color="primary"
                        variant="outlined"
                      />

                    </Box>

                    <Typography
                      variant="body2"
                      color="text.secondary"
                      sx={{ mt: 0.5 }}
                    >
                      {curso.descricao}
                    </Typography>

                  </Box>

                  <Box display="flex" gap={1}>

                    {curso.ativo ? (
                      <>
                        <Button
                          size="small"
                          variant="outlined"
                          onClick={() =>
                            navigate(`/admin/academico/cursos/${curso.id}`)
                          }
                        >
                          Gerenciar
                        </Button>

                        <Button
                          size="small"
                          color="error"
                          onClick={() => desativarCurso(curso.id)}
                        >
                          Desativar
                        </Button>
                      </>
                    ) : (
                      <Button
                        size="small"
                        color="success"
                        onClick={() => reativarCurso(curso.id)}
                      >
                        Reativar
                      </Button>
                    )}

                  </Box>

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