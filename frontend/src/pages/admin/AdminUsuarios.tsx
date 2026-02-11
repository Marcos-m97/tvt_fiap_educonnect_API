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
  CircularProgress
} from "@mui/material";
import AddIcon from "@mui/icons-material/Add";
import AppLayout from "../../components/layout/AppLayout";
import { useEffect, useState } from "react";
import { api } from "../../services/api";

interface Usuario {
  id: number;
  nome: string;
  email: string;
  tipo: number;
  ativo: boolean;
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
  const [usuarios, setUsuarios] = useState<Usuario[]>([]);
  const [loading, setLoading] = useState(false);

  const [page, setPage] = useState(1);
  const [total, setTotal] = useState(0);
  const pageSize = 5;

  const [search, setSearch] = useState("");

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
      <Typography variant="h4" gutterBottom>
        Gestão de Usuários
      </Typography>

      <Typography variant="body1" color="text.secondary" mb={3}>
        Crie, edite e gerencie administradores, professores e alunos.
      </Typography>

      {/* Ações superiores */}
      <Box
        display="flex"
        justifyContent="space-between"
        alignItems="center"
        mb={3}
        gap={2}
        flexWrap="wrap"
      >
        <Button
          variant="contained"
          startIcon={<AddIcon />}
        >
          Criar Usuário
        </Button>

        <TextField
          size="small"
          label="Pesquisar por nome, email ou ID"
          value={search}
          onChange={(e) => {
            setPage(1);
            setSearch(e.target.value);
          }}
        />
      </Box>

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
                  sx={{ opacity: usuario.ativo ? 1 : 0.5 }}
                >
                  <Box>
                    <Typography fontWeight={600}>
                      {usuario.nome}
                    </Typography>

                    <Typography variant="body2" color="text.secondary">
                      {usuario.email}
                    </Typography>

                    <Box mt={1} display="flex" gap={1}>
                      <Chip
                        label={traduzirTipo(usuario.tipo)}
                        size="small"
                      />

                      <Chip
                        label={usuario.ativo ? "Ativo" : "Inativo"}
                        color={usuario.ativo ? "success" : "default"}
                        size="small"
                      />
                    </Box>
                  </Box>

                  <Button
                    size="small"
                    onClick={() => toggleStatus(usuario)}
                  >
                    {usuario.ativo ? "Desativar" : "Reativar"}
                  </Button>
                </Box>

                {index !== usuarios.length - 1 && <Divider />}
              </Box>
            ))}
        </CardContent>

        {/* Paginação */}
        <CardActions
          sx={{ justifyContent: "center", py: 2 }}
        >
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