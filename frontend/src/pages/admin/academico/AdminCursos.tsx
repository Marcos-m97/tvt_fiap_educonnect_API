import {
  Typography,
  Box,
  Card,
  CardContent,
  CardActions,
  Button,
  TextField,
  Pagination,
  CircularProgress,
  Chip
} from "@mui/material";
import AddIcon from "@mui/icons-material/Add";
import ArrowBackIcon from "@mui/icons-material/ArrowBack";
import SchoolIcon from "@mui/icons-material/School";
import MenuBookIcon from "@mui/icons-material/MenuBook";
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

            <SchoolIcon
              sx={{
                fontSize: 60,
                color: "primary.main",
                mb: 1
              }}
            />

            <Typography variant="h4" fontWeight={700} gutterBottom>
              Gestão Acadêmica
            </Typography>

            <Typography
              variant="body1"
              color="text.secondary"
              sx={{ lineHeight: 1.7 }}
            >
              Busque um curso para gerenciar turmas e disciplinas.
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
            cursos.map((curso) => (
              <Card
                key={curso.id}
                sx={{
                  mb: 2,
                  opacity: curso.ativo ? 1 : 0.5,
                  transition: "0.25s",
                  "&:hover": {
                    transform: "translateY(-2px)",
                    boxShadow: 3
                  }
                }}
              >
                <CardContent>

                  <Box
                    display="flex"
                    justifyContent="space-between"
                    alignItems="center"
                  >

                    <Box display="flex" alignItems="center" gap={2}>

                      <MenuBookIcon color="primary" />

                      <Box display="flex" alignItems="center" gap={1}>

                        <Typography
                          fontWeight={700}
                          sx={{
                            textTransform: "uppercase",
                            letterSpacing: 0.5
                          }}
                        >
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

                    </Box>

                    <Box display="flex" gap={1}>

                      {curso.ativo ? (
                        <>
                          <Button
                            size="small"
                            variant="contained"
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

                </CardContent>
              </Card>
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