import {
  Typography,
  Box,
  Card,
  CardContent,
  Chip,
  Button,
  Divider,
  CircularProgress
} from "@mui/material";
import ArrowBackIcon from "@mui/icons-material/ArrowBack";
import AppLayout from "../../components/layout/AppLayout";
import { useParams, useNavigate } from "react-router-dom";
import { useEffect, useState } from "react";
import { api } from "../../services/api";

interface Usuario {
  id: number;
  nome: string;
  email: string;
  tipo: number;
  ativo: boolean;
  criadoEm: string;
}

export default function AdminUsuarioPerfil() {
  const { id } = useParams();
  const navigate = useNavigate();

  const [usuario, setUsuario] = useState<Usuario | null>(null);
  const [perfil, setPerfil] = useState<any>(null);
  const [contexto, setContexto] = useState<any>(null);
  const [loading, setLoading] = useState(true);

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

  async function carregarDados() {
    try {
      setLoading(true);

      const response = await api.get(`/usuario/${id}`);
      const user = response.data;
      setUsuario(user);

      if (user.tipo === 2) {
        const prof = await api.get(`/professor/${user.id}`);
        setPerfil(prof.data);

        const ctx = await api.get(`/professor/${prof.data.id}/contexto`);
        setContexto(ctx.data);
      }

      if (user.tipo === 3) {
        const aluno = await api.get(`/aluno/${user.id}`);
        setPerfil(aluno.data);

        const ctx = await api.get(`/aluno/${aluno.data.id}/contexto`);
        setContexto(ctx.data);
      }

    } catch (error) {
      console.error("Erro ao carregar perfil:", error);
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    carregarDados();
  }, [id]);

  return (
    <AppLayout>

      {/* HEADER CENTRALIZADO */}
      <Box position="relative" mb={4}>
        <Typography
          variant="h4"
          fontWeight={600}
          textAlign="center"
        >
          Perfil do Usuário
        </Typography>

        <Box position="absolute" right={0} top={0}>
          <Button
            variant="outlined"
            startIcon={<ArrowBackIcon />}
            onClick={() => navigate("/admin/usuarios")}
          >
            Voltar
          </Button>
        </Box>
      </Box>

      {loading && (
        <Box display="flex" justifyContent="center" py={4}>
          <CircularProgress />
        </Box>
      )}

      {!loading && usuario && (
        <Card
          sx={{
            borderRadius: 3,
            boxShadow: 3,
            p: 2
          }}
        >
          <CardContent>

            {/* BLOCO SUPERIOR CENTRALIZADO */}
            <Box textAlign="center" mb={3}>
              <Typography variant="h5" fontWeight={600}>
                {usuario.nome}
              </Typography>

              <Typography
                variant="body2"
                color="text.secondary"
                mb={2}
              >
                {usuario.email}
              </Typography>

              <Box display="flex" justifyContent="center" gap={1}>
                <Chip label={traduzirTipo(usuario.tipo)} />
                <Chip
                  label={usuario.ativo ? "Ativo" : "Inativo"}
                  color={usuario.ativo ? "success" : "default"}
                />
              </Box>
            </Box>

            <Divider sx={{ my: 3 }} />

            {/* DADOS DE CADASTRO */}
            {perfil && (
              <>
                <Typography variant="subtitle1" fontWeight={600} mb={2}>
                  Dados de Cadastro
                </Typography>

                <Box
                  display="grid"
                  gridTemplateColumns={{
                    xs: "1fr",
                    md: "1fr 1fr"
                  }}
                  gap={2}
                >
                  {Object.entries(perfil).map(([key, value]) => (
                    <Box
                      key={key}
                      sx={{
                        backgroundColor: "background.paper",
                        p: 2,
                        borderRadius: 2,
                        border: 1,
                        borderColor: "divider"
                      }}
                    >
                      <Typography variant="body2">
                        <strong>{key}:</strong> {String(value)}
                      </Typography>
                    </Box>
                  ))}
                </Box>

                <Divider sx={{ my: 3 }} />
              </>
            )}

            {/* CONTEXTO */}
            {contexto && (
              <>
                <Typography variant="subtitle1" fontWeight={600} mb={2}>
                  Contexto Acadêmico
                </Typography>

                {/* PROFESSOR */}
                {usuario.tipo === 2 && contexto.turmasDisciplinas && (
                  <Box display="grid" gap={2}>
                    {contexto.turmasDisciplinas.map((item: any) => (
                      <Box
                        key={item.turmaDisciplinaId}
                        sx={{
                          backgroundColor: "action.hover",
                          p: 2,
                          borderRadius: 2,
                          border: 1,
                          borderColor: "divider"
                        }}
                      >
                        <Typography fontWeight={600}>
                          {item.disciplinaNome}
                        </Typography>
                        <Typography
                          variant="body2"
                          color="text.secondary"
                        >
                          Turma: {item.turmaNome}
                        </Typography>
                      </Box>
                    ))}
                  </Box>
                )}

                {/* ALUNO */}
                {usuario.tipo === 3 && (
                  <Box display="grid" gap={2}>

                    {contexto.turmaNome && (
                      <Box
                        sx={{
                          backgroundColor: "action.hover",
                          p: 2,
                          borderRadius: 2,
                          border: 1,
                          borderColor: "divider"
                        }}
                      >
                        <Typography fontWeight={600}>
                          Turma: {contexto.turmaNome}
                        </Typography>
                        <Typography variant="body2">
                          Curso: {contexto.cursoNome}
                        </Typography>
                      </Box>
                    )}

                    {contexto.disciplinas &&
                      contexto.disciplinas.length > 0 && (
                        <Box>
                          <Typography fontWeight={600} mb={1}>
                            Disciplinas
                          </Typography>

                          <Box display="grid" gap={2}>
                            {contexto.disciplinas.map(
                              (disc: any, i: number) => (
                                <Box
                                  key={i}
                                  sx={{
                                    backgroundColor: "background.paper",
                                    p: 2,
                                    borderRadius: 2,
                                    border: 1,
                                    borderColor: "divider"
                                  }}
                                >
                                  {disc.nome ||
                                    JSON.stringify(disc)}
                                </Box>
                              )
                            )}
                          </Box>
                        </Box>
                      )}
                  </Box>
                )}
              </>
            )}

          </CardContent>
        </Card>
      )}

    </AppLayout>
  );
}
