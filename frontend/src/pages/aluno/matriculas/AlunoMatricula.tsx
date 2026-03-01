import {
  Typography,
  Box,
  Button,
  Card,
  CardContent,
  Divider,
  MenuItem,
  TextField,
  Chip,
  CircularProgress,
  Stack
} from "@mui/material";
import ArrowBackIcon from "@mui/icons-material/ArrowBack";
import UploadFileIcon from "@mui/icons-material/UploadFile";
import DownloadIcon from "@mui/icons-material/Download";
import AppLayout from "../../../components/layout/AppLayout";
import { useEffect, useState } from "react";
import { api } from "../../../services/api";
import { useNavigate } from "react-router-dom";

interface Curso {
  id: number;
  nome: string;
  ativo: boolean;
}

interface CursoResponse {
  data: Curso[];
  total: number;
}

interface Turma {
  id: number;
  nome: string;
  periodo: string;
  semestre: string;
}

interface Matricula {
  id: number;
  status: number;
}

export default function AlunoMatricula() {
  const navigate = useNavigate();

  const [loading, setLoading] = useState(true);
  const [matricula, setMatricula] = useState<Matricula | null>(null);

  const [cursos, setCursos] = useState<Curso[]>([]);
  const [cursoSelecionado, setCursoSelecionado] = useState<number | "">("");
  const [turmas, setTurmas] = useState<Turma[]>([]);
  const [turmaSelecionada, setTurmaSelecionada] = useState<number | "">("");

  const [uploading, setUploading] = useState(false);

  useEffect(() => {
    async function carregar() {
      try {
        const me = await api.get("/account/me");
        const alunoId = me.data.perfil.id;

        try {
          const mat = await api.get(`/matricula/aluno/${alunoId}`);
          if (mat.data?.length > 0) {
            setMatricula(mat.data[0]);
          }
        } catch {}

        const cursosRes = await api.get<CursoResponse>(
          "/curso?page=1&pageSize=50"
        );

        setCursos(
          cursosRes.data.data.filter((c) => c.ativo)
        );

      } catch (error) {
        console.error(error);
      } finally {
        setLoading(false);
      }
    }

    carregar();
  }, []);

  useEffect(() => {
    if (!cursoSelecionado) return;

    async function carregarTurmas() {
      const res = await api.get(
        `/turma/curso/${cursoSelecionado}`
      );
      setTurmas(res.data);
    }

    carregarTurmas();
  }, [cursoSelecionado]);

  async function iniciarMatricula() {
    if (!turmaSelecionada) return;

    const res = await api.post("/matricula", {
      turmaId: turmaSelecionada
    });

    setMatricula(res.data);
  }

  async function baixarArquivo(tipo: string) {
    if (!matricula) return;

    try {
      const response = await api.get(
        `/matricula/${matricula.id}/download/${tipo}`,
        { responseType: "blob" }
      );

      const url = window.URL.createObjectURL(
        new Blob([response.data])
      );

      const link = document.createElement("a");
      link.href = url;
      link.setAttribute("download", `${tipo}.pdf`);
      document.body.appendChild(link);
      link.click();
    } catch {
      alert("Arquivo não encontrado.");
    }
  }

  async function uploadArquivo(tipo: string, file: File) {
    if (!matricula) return;

    const formData = new FormData();
    formData.append("arquivo", file);

    try {
      setUploading(true);
      await api.put(
        `/matricula/${matricula.id}/upload-${tipo}`,
        formData
      );
      alert("Arquivo enviado com sucesso!");
    } catch {
      alert("Erro ao enviar arquivo.");
    } finally {
      setUploading(false);
    }
  }

  function traduzirStatus(status: number) {
    const mapa: Record<number, string> = {
      1: "Inscrição",
      2: "Pagamento",
      3: "Documentos",
      4: "Efetivada"
    };
    return mapa[status] ?? "Desconhecido";
  }

  function getStatusColor(status: number) {
    switch (status) {
      case 4:
        return "success";
      case 2:
        return "warning";
      case 3:
        return "info";
      default:
        return "default";
    }
  }

  if (loading) {
    return (
      <AppLayout>
        <Box display="flex" justifyContent="center" mt={10}>
          <CircularProgress />
        </Box>
      </AppLayout>
    );
  }

  return (
    <AppLayout>
      <Box maxWidth="950px" mx="auto" mt={6}>

        {/* TÍTULO */}
        <Box textAlign="center" mb={6}>
          <Typography variant="h3" fontWeight={800}>
            Minha Matrícula
          </Typography>
        </Box>

        <Card
          sx={{
            borderRadius: 4,
            boxShadow: 3,
            p: 2
          }}
        >
          <CardContent sx={{ p: 4 }}>

            {/* HEADER CARD */}
            <Box
              display="flex"
              justifyContent="space-between"
              alignItems="center"
              mb={3}
            >
              {matricula && (
                <Box display="flex" alignItems="center" gap={2}>
                  <Typography fontWeight={600}>
                    Status Atual:
                  </Typography>
                  <Chip
                    label={traduzirStatus(matricula.status)}
                    color={getStatusColor(matricula.status)}
                  />
                </Box>
              )}

              {matricula?.status === 4 && (
                <Button
                  variant="outlined"
                  startIcon={<ArrowBackIcon />}
                  onClick={() => navigate("/aluno")}
                >
                  Voltar
                </Button>
              )}
            </Box>

            <Divider sx={{ mb: 4 }} />

            {/* SE NÃO TEM MATRÍCULA */}
            {!matricula && (
              <Stack spacing={3}>
                <TextField
                  select
                  label="Curso"
                  value={cursoSelecionado}
                  onChange={(e) =>
                    setCursoSelecionado(Number(e.target.value))
                  }
                  fullWidth
                >
                  {cursos.map((curso) => (
                    <MenuItem key={curso.id} value={curso.id}>
                      {curso.nome}
                    </MenuItem>
                  ))}
                </TextField>

                <TextField
                  select
                  label="Turma"
                  value={turmaSelecionada}
                  onChange={(e) =>
                    setTurmaSelecionada(Number(e.target.value))
                  }
                  fullWidth
                  disabled={!cursoSelecionado}
                >
                  {turmas.map((turma) => (
                    <MenuItem key={turma.id} value={turma.id}>
                      {turma.nome} • {turma.periodo} • {turma.semestre}
                    </MenuItem>
                  ))}
                </TextField>

                <Button
                  variant="contained"
                  onClick={iniciarMatricula}
                  disabled={!turmaSelecionada}
                >
                  Iniciar Matrícula
                </Button>
              </Stack>
            )}

            {/* DOCUMENTOS */}
            {matricula && (
              <>
                <Typography
                  variant="h6"
                  fontWeight={700}
                  mb={4}
                >
                  Documentos
                </Typography>

                <Stack spacing={3}>
                  {[
                    {
                      titulo: "Comprovante de Pagamento",
                      tipo: "comprovante"
                    },
                    {
                      titulo: "Documentos Pessoais",
                      tipo: "documentos-pessoais"
                    },
                    {
                      titulo: "Documentos Escolaridade",
                      tipo: "documentos-escolaridade"
                    }
                  ].map((doc) => (
                    <Box
                      key={doc.tipo}
                      display="flex"
                      justifyContent="space-between"
                      alignItems="center"
                      sx={{
                        p: 2,
                        border: "1px solid",
                        borderColor: "divider",
                        borderRadius: 2
                      }}
                    >
                      <Typography fontWeight={600}>
                        {doc.titulo}
                      </Typography>

                      <Box display="flex" gap={2}>
                        <Button
                          component="label"
                          startIcon={<UploadFileIcon />}
                          variant="outlined"
                          size="small"
                          disabled={uploading}
                        >
                          Upload
                          <input
                            type="file"
                            hidden
                            onChange={(e) =>
                              e.target.files &&
                              uploadArquivo(doc.tipo, e.target.files[0])
                            }
                          />
                        </Button>

                        <Button
                          startIcon={<DownloadIcon />}
                          variant="outlined"
                          size="small"
                          onClick={() =>
                            baixarArquivo(doc.tipo)
                          }
                        >
                          Download
                        </Button>
                      </Box>
                    </Box>
                  ))}
                </Stack>
              </>
            )}

          </CardContent>
        </Card>

      </Box>
    </AppLayout>
  );
}