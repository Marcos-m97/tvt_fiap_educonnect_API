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
import AppLayout from "../../components/layout/AppLayout";
import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { api } from "../../services/api";

interface Usuario {
  id: number;
  nome: string;
  email: string;
  tipo: number;
}

export default function MeuPerfil() {
  const navigate = useNavigate();

  const [loading, setLoading] = useState(true);
  const [usuario, setUsuario] = useState<Usuario | null>(null);
  const [perfil, setPerfil] = useState<any>(null);

  const [mensagem, setMensagem] = useState<string | null>(null);
  const [erro, setErro] = useState(false);

  async function carregar() {
    try {
      setLoading(true);

      const response = await api.get("/account/me");

      setUsuario(response.data.usuario);
      setPerfil(response.data.perfil);
    } catch (error) {
      console.error("Erro ao carregar perfil:", error);
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    carregar();
  }, []);

  async function handleSubmit() {
    if (!usuario || !perfil) return;

    try {
      setLoading(true);
      setMensagem(null);

      // Atualiza dados do usuário
      await api.put(`/usuario/${usuario.id}`, {
        nome: usuario.nome,
        email: usuario.email,
        tipo: usuario.tipo
      });

      // Atualiza dados do perfil conforme tipo
      if (usuario.tipo === 1) {
        await api.put(`/admin/${perfil.id}`, {
          usuarioId: usuario.id,
          departamento: perfil.departamento,
          cargo: perfil.cargo
        });
      }

      if (usuario.tipo === 2) {
        await api.put(`/professor/${perfil.id}`, {
          usuarioId: usuario.id,
          especialidade: perfil.especialidade,
          formacao: perfil.formacao,
          curriculoLattes: perfil.curriculoLattes
        });
      }

      if (usuario.tipo === 3) {
        await api.put(`/aluno/${perfil.id}`, {
          usuarioId: usuario.id,
          cpf: perfil.cpf,
          dataNascimento: perfil.dataNascimento,
          endereco: perfil.endereco
        });
      }

      setErro(false);
      setMensagem("Perfil atualizado com sucesso!");

      setTimeout(() => {
        setMensagem(null);
      }, 4000);

    } catch (error) {
      console.error("Erro ao atualizar perfil:", error);
      setErro(true);
      setMensagem("Erro ao atualizar perfil.");

      setTimeout(() => {
        setMensagem(null);
      }, 4000);
    } finally {
      setLoading(false);
    }
  }

  return (
    <AppLayout>
      <Box mb={6} textAlign="center">
        <Typography variant="h4" fontWeight={600} gutterBottom>
          Meu Perfil
        </Typography>

        <Typography variant="body1" color="text.secondary">
          Atualize suas informações pessoais.
        </Typography>
      </Box>

      <Box maxWidth="900px" mx="auto">

        {loading && (
          <Box display="flex" justifyContent="center" py={6}>
            <CircularProgress />
          </Box>
        )}

        {!loading && usuario && (
          <Card
            sx={{
              borderRadius: 4,
              boxShadow: 5,
              px: 6,
              py: 6
            }}
          >
            <CardContent sx={{ p: 0 }}>
              <Box display="flex" flexDirection="column" gap={4}>

                <Typography fontWeight={600} variant="h6" textAlign="center">
                  Dados do Usuário
                </Typography>

                <TextField
                  label="Nome"
                  value={usuario.nome}
                  onChange={(e) =>
                    setUsuario({ ...usuario, nome: e.target.value })
                  }
                  fullWidth
                />

                <TextField
                  label="Email"
                  value={usuario.email}
                  disabled
                  fullWidth
                />

                <Divider sx={{ my: 2 }} />

                {perfil && (
                  <Box display="flex" flexDirection="column" gap={4}>
                    <Typography fontWeight={600} variant="h6" textAlign="center">
                      Dados do Perfil
                    </Typography>

                    {usuario.tipo === 1 && (
                      <>
                        <TextField
                          label="Departamento"
                          value={perfil.departamento || ""}
                          onChange={(e) =>
                            setPerfil({
                              ...perfil,
                              departamento: e.target.value
                            })
                          }
                          fullWidth
                        />
                        <TextField
                          label="Cargo"
                          value={perfil.cargo || ""}
                          onChange={(e) =>
                            setPerfil({
                              ...perfil,
                              cargo: e.target.value
                            })
                          }
                          fullWidth
                        />
                      </>
                    )}

                    {usuario.tipo === 2 && (
                      <>
                        <TextField
                          label="Especialidade"
                          value={perfil.especialidade || ""}
                          onChange={(e) =>
                            setPerfil({
                              ...perfil,
                              especialidade: e.target.value
                            })
                          }
                          fullWidth
                        />
                        <TextField
                          label="Formação"
                          value={perfil.formacao || ""}
                          onChange={(e) =>
                            setPerfil({
                              ...perfil,
                              formacao: e.target.value
                            })
                          }
                          fullWidth
                        />
                        <TextField
                          label="Currículo Lattes"
                          multiline
                          rows={3}
                          value={perfil.curriculoLattes || ""}
                          onChange={(e) =>
                            setPerfil({
                              ...perfil,
                              curriculoLattes: e.target.value
                            })
                          }
                          fullWidth
                        />
                      </>
                    )}

                    {usuario.tipo === 3 && (
                      <>
                        <TextField
                          label="CPF"
                          value={perfil.cpf || ""}
                          onChange={(e) =>
                            setPerfil({
                              ...perfil,
                              cpf: e.target.value
                            })
                          }
                          fullWidth
                        />
                        <TextField
                          label="Data de Nascimento"
                          type="date"
                          value={perfil.dataNascimento?.split("T")[0] || ""}
                          onChange={(e) =>
                            setPerfil({
                              ...perfil,
                              dataNascimento: e.target.value
                            })
                          }
                          fullWidth
                          InputLabelProps={{ shrink: true }}
                        />
                        <TextField
                          label="Endereço"
                          value={perfil.endereco || ""}
                          onChange={(e) =>
                            setPerfil({
                              ...perfil,
                              endereco: e.target.value
                            })
                          }
                          fullWidth
                        />
                      </>
                    )}
                  </Box>
                )}

                <Box display="flex" justifyContent="center" gap={3} mt={3}>
                  <Button
                    variant="outlined"
                    startIcon={<ArrowBackIcon />}
                    onClick={() => navigate(-1)}
                    sx={{ px: 5, borderRadius: 3 }}
                  >
                    Voltar
                  </Button>

                  <Button
                    startIcon={<SaveIcon />}
                    onClick={handleSubmit}
                    disabled={loading}
                    sx={{
                      px: 5,
                      borderRadius: 3,
                      background: "linear-gradient(90deg, #1976d2, #26c6da)",
                      color: "#fff",
                      "&:hover": {
                        background: "linear-gradient(90deg, #1565c0, #00acc1)"
                      }
                    }}
                  >
                    Salvar Alterações
                  </Button>
                </Box>

                {mensagem && (
                  <Box textAlign="center" mt={2}>
                    <Typography
                      variant="body2"
                      fontWeight={500}
                      color={erro ? "error.main" : "success.main"}
                    >
                      {mensagem}
                    </Typography>
                  </Box>
                )}

              </Box>
            </CardContent>
          </Card>
        )}
      </Box>
    </AppLayout>
  );
}