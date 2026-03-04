import {
  Typography,
  Box,
  Card,
  CardContent,
  Chip,
  Button,
  Divider,
  CircularProgress,
  Avatar
} from "@mui/material";
import ArrowBackIcon from "@mui/icons-material/ArrowBack";
import EditIcon from "@mui/icons-material/Edit";
import PersonIcon from "@mui/icons-material/Person";
import AppLayout from "../../../components/layout/AppLayout";
import { useParams, useNavigate } from "react-router-dom";
import { useEffect, useState } from "react";
import { api } from "../../../services/api";

interface Usuario {
  id: number;
  nome: string;
  email: string;
  tipo: number;
  ativo: boolean;
  criadoEm: string;
  fotoPerfilUrl?: string | null;
}

export default function AdminUsuarioPerfil() {
  const { id } = useParams();
  const navigate = useNavigate();

  const [usuario, setUsuario] = useState<Usuario | null>(null);
  const [perfil, setPerfil] = useState<any>(null);
  const [contexto, setContexto] = useState<any>(null);
  const [loading, setLoading] = useState(true);

  const baseUrl = api.defaults.baseURL?.replace("/api", "");

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

  function traduzirCampo(key: string) {
    switch (key) {
      case "dataNascimento":
        return "Data de Nascimento";
      case "cpf":
        return "CPF";
      case "endereco":
        return "Endereço";
      case "email":
        return "E-mail";
      case "departamento":
        return "Departamento";
      case "cargo":
        return "Cargo";
      default:
        return key;
    }
  }

  function formatarValor(key: string, value: any) {
    if (!value) return "-";

    if (key === "dataNascimento" && typeof value === "string") {
      return value.split("T")[0].split("-").reverse().join("/");
    }

    return value;
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

      else if (user.tipo === 3) {
        const aluno = await api.get(`/aluno/${user.id}`);
        setPerfil(aluno.data);

        const ctx = await api.get(`/aluno/${aluno.data.id}/contexto`);
        setContexto(ctx.data);
      }

      else if (user.tipo === 0 || user.tipo === 1) {
        const admin = await api.get(`/admin/${user.id}`);
        setPerfil(admin.data);
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

      {/* TÍTULO MAIOR */}
      <Box mb={4}>
        <Typography
          variant="h3"
          fontWeight={600}
          textAlign="center"
        >
          Perfil do Usuário
        </Typography>
      </Box>

      {loading && (
        <Box display="flex" justifyContent="center" py={4}>
          <CircularProgress />
        </Box>
      )}

      {!loading && usuario && (
        <Card sx={{ borderRadius: 4, boxShadow: 3, p: 3 }}>
          
          {/* BOTÕES AGORA DENTRO DO CARD */}
          <Box display="flex" justifyContent="flex-end" gap={2} mb={2}>
            <Button
              variant="outlined"
              startIcon={<EditIcon />}
              onClick={() => navigate(`/admin/usuarios/${id}/editar`)}
            >
              Editar
            </Button>

            <Button
              variant="outlined"
              startIcon={<ArrowBackIcon />}
              onClick={() => navigate("/admin/usuarios")}
            >
              Voltar
            </Button>
          </Box>

          <CardContent>

            {/* FOTO + NOME */}
            <Box textAlign="center" mb={4}>
              <Avatar
                src={
                  usuario.fotoPerfilUrl
                    ? `${baseUrl}${usuario.fotoPerfilUrl}`
                    : undefined
                }
                sx={{
                  width: 130,
                  height: 130,
                  mx: "auto",
                  mb: 2,
                  fontSize: 52,
                  bgcolor: "grey.400"
                }}
              >
                {!usuario.fotoPerfilUrl && <PersonIcon fontSize="large" />}
              </Avatar>

              <Typography variant="h4" fontWeight={700}>
                {usuario.nome}
              </Typography>

              <Typography variant="body1" color="text.secondary" mb={2}>
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
                <Typography variant="h6" fontWeight={600} mb={2}>
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
                  {Object.entries(perfil)
                    .filter(
                      ([key]) =>
                        !["id", "usuarioId", "nome"].includes(key)
                    )
                    .map(([key, value]) => (
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
                        <Typography variant="body1">
                          <strong>{traduzirCampo(key)}:</strong>{" "}
                          {formatarValor(key, value)}
                        </Typography>
                      </Box>
                    ))}
                </Box>

                <Divider sx={{ my: 3 }} />
              </>
            )}

            {/* CONTEXTO (mantido exatamente igual) */}
            {contexto && (
              <>
                <Typography variant="h6" fontWeight={600} mb={2}>
                  Contexto Acadêmico
                </Typography>

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
                        <Typography variant="body2" color="text.secondary">
                          Turma: {item.turmaNome}
                        </Typography>
                      </Box>
                    ))}
                  </Box>
                )}

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
                                  {disc.nome || JSON.stringify(disc)}
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