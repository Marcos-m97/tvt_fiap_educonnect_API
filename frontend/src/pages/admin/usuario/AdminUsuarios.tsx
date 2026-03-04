import {
  Typography,
  Box,
  Card,
  CardContent,
  CardActions,
  Button,
  Chip,
  Divider,
  TextField,
  Pagination,
  CircularProgress,
  Avatar
} from "@mui/material";
import AddIcon from "@mui/icons-material/Add";
import ArrowBackIcon from "@mui/icons-material/ArrowBack";
import PersonIcon from "@mui/icons-material/Person";
import AppLayout from "../../../components/layout/AppLayout";
import { useEffect, useState } from "react";
import { api } from "../../../services/api";
import { useNavigate } from "react-router-dom";

interface Usuario {
  id: number;
  nome: string;
  email: string;
  tipo: number;
  ativo: boolean;
  fotoPerfilUrl?: string | null;
}

function traduzirTipo(tipo: number) {
  switch (tipo) {
    case 0:
      return "SuperAdmin";
    case 1:
      return "Admin";
    case 2:
      return "Professor";
    case 3:
      return "Aluno";
    default:
      return "Desconhecido";
  }
}

export default function AdminUsuarios() {
  const navigate = useNavigate();

  const [usuarios, setUsuarios] = useState<Usuario[]>([]);
  const [loading, setLoading] = useState(false);

  const [page, setPage] = useState(1);
  const [total, setTotal] = useState(0);
  const pageSize = 5;

  const [search, setSearch] = useState("");

  const baseUrl = api.defaults.baseURL?.replace("/api", "");

  async function carregarUsuarios() {
    try {
      setLoading(true);

      const response = await api.get("/usuario", {
        params: {
          page,
          pageSize,
          search: search || undefined
        }
      });

      setUsuarios(response.data.data);
      setTotal(response.data.total);
    } catch (error) {
      console.error("Erro ao carregar usuários:", error);
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    carregarUsuarios();
  }, [page, search]);

  async function toggleStatus(usuario: Usuario) {
    try {
      if (usuario.ativo) {
        await api.delete(`/usuario/${usuario.id}`);
      } else {
        await api.put(`/usuario/${usuario.id}/reativar`);
      }

      carregarUsuarios();
    } catch (error) {
      console.error("Erro ao alterar status:", error);
    }
  }

  const totalPages = Math.ceil(total / pageSize);

  return (
    <AppLayout>

      {/* HEADER PADRONIZADO */}
      <Card sx={{ mb: 4 }}>
        <CardContent>

          <Box textAlign="center" maxWidth={650} mx="auto">

            <Typography variant="h4" fontWeight={700} gutterBottom>
              Gestão de Usuários
            </Typography>

            <Typography
              variant="body1"
              color="text.secondary"
              sx={{ lineHeight: 1.7 }}
            >
              Crie, edite e gerencie administradores, professores e alunos.
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
              onClick={() => navigate("/admin/usuarios/novo")}
            >
              Criar Usuário
            </Button>

            <TextField
              placeholder="Pesquisar por nome, email ou ID"
              size="small"
              value={search}
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


      {/* LISTA DE USUÁRIOS */}
      <Card>
        <CardContent>

          {loading && (
            <Box display="flex" justifyContent="center" py={3}>
              <CircularProgress size={24} />
            </Box>
          )}

          {!loading && usuarios.length === 0 && (
            <Typography variant="body2" color="text.secondary">
              Nenhum usuário encontrado.
            </Typography>
          )}

          {!loading &&
            usuarios.map((usuario, index) => (
              <Box key={usuario.id}>

                <Box
                  display="flex"
                  justifyContent="space-between"
                  alignItems="center"
                  py={2}
                  sx={{
                    opacity: usuario.ativo ? 1 : 0.45,
                    transition: "0.25s",
                    "&:hover": {
                      background: "rgba(0,0,0,0.03)",
                      borderRadius: 2,
                      px: 1
                    }
                  }}
                >

                  {/* LADO ESQUERDO */}
                  <Box display="flex" alignItems="center" gap={3}>

                    <Avatar
                      src={
                        usuario.fotoPerfilUrl
                          ? `${baseUrl}${usuario.fotoPerfilUrl}`
                          : undefined
                      }
                      sx={{
                        width: 52,
                        height: 52,
                        bgcolor: "grey.400"
                      }}
                    >
                      {!usuario.fotoPerfilUrl && <PersonIcon />}
                    </Avatar>

                    <Box>

                      <Typography fontWeight={600}>
                        {usuario.nome}
                      </Typography>

                      <Typography variant="body2" color="text.secondary">
                        {usuario.email}
                      </Typography>

                      <Box mt={1} display="flex" gap={1} flexWrap="wrap">

                        <Chip
                          label={traduzirTipo(usuario.tipo)}
                          size="small"
                        />

                        <Chip
                          label={usuario.ativo ? "Ativo" : "Inativo"}
                          size="small"
                          color={usuario.ativo ? "success" : "default"}
                        />

                      </Box>

                    </Box>

                  </Box>

                  {/* BOTÕES */}
                  <Box display="flex" gap={1}>

                    <Button
                      size="small"
                      variant="outlined"
                      onClick={() =>
                        navigate(`/admin/usuarios/${usuario.id}`)
                      }
                    >
                      Ver Perfil
                    </Button>

                    <Button
                      size="small"
                      onClick={() => toggleStatus(usuario)}
                    >
                      {usuario.ativo ? "Desativar" : "Reativar"}
                    </Button>

                  </Box>

                </Box>

                {index !== usuarios.length - 1 && <Divider />}

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