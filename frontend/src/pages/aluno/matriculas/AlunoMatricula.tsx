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

import Stepper from "@mui/material/Stepper";
import Step from "@mui/material/Step";
import StepLabel from "@mui/material/StepLabel";

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

  const passos = [
    "Inscrição",
    "Pagamento",
    "Documentos",
    "Efetivada"
  ];

  function getStep(status: number) {
    switch (status) {
      case 1: return 0;
      case 2: return 1;
      case 3: return 2;
      case 4: return 3;
      default: return 0;
    }
  }

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

      case 4: return "success";
      case 2: return "warning";
      case 3: return "info";
      default: return "default";

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

        <Box textAlign="center" mb={4}>

          <Typography variant="h3" fontWeight={800}>
            Minha Matrícula
          </Typography>

        </Box>

        {!matricula && (

          <Card
            sx={{
              borderRadius: 3,
              mb: 4,
              background: "linear-gradient(135deg,#1976d2 0%,#26c6da 100%)",
              color: "white"
            }}
          >

            <CardContent sx={{ p: 4 }}>

              <Typography
                variant="h5"
                fontWeight={700}
                mb={1}
              >
                Inicie sua Matrícula
              </Typography>

              <Typography
                variant="body1"
                sx={{ opacity: 0.9, lineHeight: 1.7 }}
              >

                Selecione o curso e a turma desejada para iniciar
                seu processo de matrícula no EduConnect.

                Após a inscrição, será necessário enviar os documentos
                obrigatórios para validação da secretaria acadêmica.

              </Typography>

            </CardContent>

          </Card>

        )}

        {matricula && (

  <Box
    sx={{
      mb: 5,
      mt: 2
    }}
  >

    <Stepper
      activeStep={getStep(matricula.status)}
      alternativeLabel
    >

      {passos.map((label) => (

        <Step key={label}>
          <StepLabel>{label}</StepLabel>
        </Step>

      ))}

    </Stepper>

  </Box>

)}

        <Card
          sx={{
            borderRadius: 3,
            boxShadow: "0px 6px 24px rgba(0,0,0,0.08)"
          }}
        >

          <CardContent sx={{ p: 4 }}>

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
                  size="large"
                  sx={{
                    mt: 2,
                    height: 48,
                    fontWeight: 600
                  }}
                  onClick={iniciarMatricula}
                  disabled={!turmaSelecionada}
                >
                  Iniciar Matrícula
                </Button>

              </Stack>

            )}

            {matricula && (

              <>

                <Box mb={3}>

                  <Typography variant="h6" fontWeight={700} mb={1}>
                    Envio de Documentos
                  </Typography>

                  <Typography
                    variant="body2"
                    color="text.secondary"
                    sx={{ lineHeight: 1.7 }}
                  >

                    Para efetivar sua matrícula é necessário enviar os
                    documentos solicitados abaixo.

                    <br /><br />

                    • Todos os arquivos devem estar no formato <strong>PDF</strong>  
                    • Certifique-se que os documentos estão legíveis  
                    • Após o envio, a secretaria acadêmica irá validar os arquivos

                  </Typography>

                </Box>

                <Divider sx={{ mb: 3 }} />

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
                      flexWrap="wrap"
                      gap={2}
                      sx={{
                        p: 2.5,
                        border: "1px solid",
                        borderColor: "divider",
                        borderRadius: 2,
                        background: "rgba(0,0,0,0.02)"
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