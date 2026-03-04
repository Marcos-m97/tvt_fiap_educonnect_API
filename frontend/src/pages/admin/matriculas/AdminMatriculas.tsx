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
  Chip,
  MenuItem
} from "@mui/material";
import ArrowBackIcon from "@mui/icons-material/ArrowBack";
import AppLayout from "../../../components/layout/AppLayout";
import { useEffect, useState } from "react";
import { api } from "../../../services/api";
import { useNavigate } from "react-router-dom";

interface Matricula {
  id: number;
  alunoNome: string;
  turmaNome: string;
  status: number | string;
  criadoEm: string;
}

export default function AdminMatriculas() {
  const navigate = useNavigate();

  const [matriculas, setMatriculas] = useState<Matricula[]>([]);
  const [loading, setLoading] = useState(false);

  const [page, setPage] = useState(1);
  const [total, setTotal] = useState(0);
  const pageSize = 10;

  const [search, setSearch] = useState("");
  const [status, setStatus] = useState("");

  async function carregarMatriculas() {
    try {
      setLoading(true);

      const response = await api.get("/matricula", {
        params: {
          page,
          pageSize,
          search: search || undefined,
          status: status || undefined
        }
      });

      setMatriculas(response.data.data);
      setTotal(response.data.total);

    } catch (error) {
      console.error("Erro ao carregar matrículas:", error);
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    carregarMatriculas();
  }, [page, search, status]);

  const totalPages = Math.ceil(total / pageSize);

  function traduzirStatus(status: number | string): string {
    const mapa: Record<number, string> = {
      0: "Inativa",
      1: "Inscrição",
      2: "Pagamento",
      3: "Documentos",
      4: "Efetivada"
    };

    if (typeof status === "number") {
      return mapa[status] ?? "Desconhecido";
    }

    return status;
  }

  function getStatusColor(status: string) {
    switch (status) {
      case "Efetivada":
        return "success";
      case "Pagamento":
        return "warning";
      case "Documentos":
        return "info";
      case "Inativa":
        return "error";
      default:
        return "default";
    }
  }

  return (
    <AppLayout>

      {/* HEADER */}
      <Card sx={{ mb: 4 }}>
        <CardContent>

          <Box textAlign="center" maxWidth={650} mx="auto">

            <Typography variant="h4" fontWeight={700} gutterBottom>
              Gestão de Matrículas
            </Typography>

            <Typography
              variant="body1"
              color="text.secondary"
              sx={{ lineHeight: 1.7 }}
            >
              Gerencie solicitações, documentos e efetivação de matrículas.
            </Typography>

          </Box>

          {/* FILTROS */}
          <Box
            display="flex"
            justifyContent="center"
            alignItems="center"
            gap={2}
            mt={3}
            flexWrap="wrap"
          >

            <TextField
              placeholder="Pesquisar aluno..."
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

            <TextField
              select
              label="Status"
              size="small"
              value={status}
              onChange={(e) => {
                setPage(1);
                setStatus(e.target.value);
              }}
              sx={{ width: 200 }}
            >
              <MenuItem value="">Todos</MenuItem>
              <MenuItem value="Inscricao">Inscrição</MenuItem>
              <MenuItem value="Pagamento">Pagamento</MenuItem>
              <MenuItem value="Documentos">Documentos</MenuItem>
              <MenuItem value="Efetivada">Efetivada</MenuItem>
              <MenuItem value="Inativa">Inativa</MenuItem>
            </TextField>

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


      {/* LISTA DE MATRÍCULAS */}
      <Card>
        <CardContent>

          {loading && (
            <Box display="flex" justifyContent="center" py={3}>
              <CircularProgress size={24} />
            </Box>
          )}

          {!loading && matriculas.length === 0 && (
            <Typography variant="body2" color="text.secondary">
              Nenhuma matrícula encontrada.
            </Typography>
          )}

          {!loading &&
            matriculas.map((m, index) => {
              const statusFormatado = traduzirStatus(m.status);

              return (
                <Box key={m.id}>

                  <Box
                    display="flex"
                    justifyContent="space-between"
                    alignItems="center"
                    py={2}
                    sx={{
                      transition: "0.25s",
                      "&:hover": {
                        background: "rgba(0,0,0,0.03)",
                        borderRadius: 2,
                        px: 1
                      }
                    }}
                  >

                    {/* INFORMAÇÕES */}
                    <Box>

                      <Typography fontWeight={600}>
                        {m.alunoNome}
                      </Typography>

                      <Box
                        mt={1}
                        display="flex"
                        alignItems="center"
                        gap={3}
                        flexWrap="wrap"
                      >

                        <Typography variant="body2" color="text.secondary">
                          Turma: {m.turmaNome}
                        </Typography>

                        <Typography variant="body2" color="text.secondary">
                          Criado em: {new Date(m.criadoEm).toLocaleDateString()}
                        </Typography>

                        <Box display="flex" alignItems="center" gap={1}>
                          <Typography variant="body2" color="text.secondary">
                            Fase:
                          </Typography>

                          <Chip
                            label={statusFormatado}
                            size="small"
                            color={getStatusColor(statusFormatado)}
                          />
                        </Box>

                      </Box>

                    </Box>

                    {/* BOTÃO */}
                    <Button
                      size="small"
                      variant="outlined"
                      onClick={() =>
                        navigate(`/admin/matriculas/${m.id}`)
                      }
                    >
                      Gerenciar
                    </Button>

                  </Box>

                  {index !== matriculas.length - 1 && <Divider />}

                </Box>
              );
            })}

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