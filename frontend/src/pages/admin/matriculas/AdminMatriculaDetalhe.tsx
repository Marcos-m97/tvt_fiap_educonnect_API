import {
  Typography,
  Box,
  Button,
  Card,
  CardContent,
  Divider,
  MenuItem,
  TextField,
  Chip
} from "@mui/material";
import ArrowBackIcon from "@mui/icons-material/ArrowBack";
import DownloadIcon from "@mui/icons-material/Download";
import AppLayout from "../../../components/layout/AppLayout";
import { useEffect, useState } from "react";
import { api } from "../../../services/api";
import { useNavigate, useParams } from "react-router-dom";

interface Matricula {
  id: number;
  alunoNome: string;
  turmaNome: string;
  status: number;
  criadoEm: string;
}

export default function AdminMatriculaDetalhe() {
  const navigate = useNavigate();
  const { id } = useParams();

  const [matricula, setMatricula] = useState<Matricula | null>(null);
  const [novoStatus, setNovoStatus] = useState<number | "">("");

  async function carregar() {
    try {
      const response = await api.get(`/matricula/${id}`);
      setMatricula(response.data);
      setNovoStatus(response.data.status);
    } catch (error) {
      console.error("Erro ao carregar matrícula:", error);
    }
  }

  useEffect(() => {
    carregar();
  }, []);

  async function alterarStatus() {
    if (novoStatus === "") return;
    await api.put(`/matricula/${id}/status/${novoStatus}`);
    carregar();
  }

  async function baixarArquivo(tipo: string) {
    try {
      const response = await api.get(
        `/matricula/${id}/download/${tipo}`,
        { responseType: "blob" }
      );

      const url = window.URL.createObjectURL(new Blob([response.data]));
      const link = document.createElement("a");
      link.href = url;
      link.setAttribute("download", `${tipo}.pdf`);
      document.body.appendChild(link);
      link.click();
    } catch {
      alert("Arquivo não encontrado.");
    }
  }

  function traduzirStatus(status: number) {
    const mapa: Record<number, string> = {
      0: "Inativa",
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
      case 0:
        return "error";
      default:
        return "default";
    }
  }

  if (!matricula) return null;

  return (
    <AppLayout>

      {/* HEADER */}
      <Box
        display="flex"
        justifyContent="space-between"
        alignItems="center"
        mb={4}
      >
        <Box>
          <Typography variant="h4" gutterBottom>
            Gerenciar Matrícula
          </Typography>

          {/* 🔥 Nome maior */}
          <Typography variant="h6" fontWeight={600}>
            {matricula.alunoNome}
          </Typography>

          {/* 🔥 Turma em chip */}
          <Chip
            label={matricula.turmaNome}
            size="small"
            sx={{ mt: 1 }}
          />
        </Box>

        <Button
          variant="outlined"
          startIcon={<ArrowBackIcon />}
          onClick={() => navigate("/admin/matriculas")}
        >
          Voltar
        </Button>
      </Box>

      <Card>
        <CardContent>

          {/* 🔥 STATUS ATUAL NA MESMA LINHA */}
          <Box
            display="flex"
            alignItems="center"
            gap={2}
          >
            <Typography variant="subtitle1" fontWeight={500}>
              Status Atual:
            </Typography>

            <Chip
              label={traduzirStatus(matricula.status)}
              color={getStatusColor(matricula.status)}
            />
          </Box>

          <Divider sx={{ my: 3 }} />

          {/* ALTERAR STATUS */}
          <Box mb={3}>
            <Typography variant="subtitle2" gutterBottom>
              Alterar Status
            </Typography>

            <Box display="flex" gap={2} alignItems="center">
              <TextField
                select
                value={novoStatus}
                onChange={(e) =>
                  setNovoStatus(Number(e.target.value))
                }
                sx={{ width: 250 }}
              >
                <MenuItem value={0}>Inativa</MenuItem>
                <MenuItem value={1}>Inscrição</MenuItem>
                <MenuItem value={2}>Pagamento</MenuItem>
                <MenuItem value={3}>Documentos</MenuItem>
                <MenuItem value={4}>Efetivada</MenuItem>
              </TextField>

              <Button
                variant="contained"
                onClick={alterarStatus}
              >
                Salvar
              </Button>
            </Box>
          </Box>

          <Divider sx={{ my: 3 }} />

          {/* DOCUMENTOS */}
          <Box>
            <Typography variant="subtitle2" gutterBottom>
              Documentos Enviados
            </Typography>

            <Box display="flex" gap={2} flexWrap="wrap">
              <Button
                startIcon={<DownloadIcon />}
                variant="outlined"
                onClick={() => baixarArquivo("comprovante")}
              >
                Comprovante Pagamento
              </Button>

              <Button
                startIcon={<DownloadIcon />}
                variant="outlined"
                onClick={() =>
                  baixarArquivo("documentos-pessoais")
                }
              >
                Documentos Pessoais
              </Button>

              <Button
                startIcon={<DownloadIcon />}
                variant="outlined"
                onClick={() =>
                  baixarArquivo("documentos-escolaridade")
                }
              >
                Documentos Escolaridade
              </Button>
            </Box>
          </Box>

        </CardContent>
      </Card>

    </AppLayout>
  );
}