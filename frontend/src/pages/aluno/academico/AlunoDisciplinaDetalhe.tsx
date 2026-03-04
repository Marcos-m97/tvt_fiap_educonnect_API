import {
  Typography,
  Box,
  Card,
  CardContent,
  Button,
  Divider,
  CircularProgress,
  Tabs,
  Tab
} from "@mui/material";
import ArrowBackIcon from "@mui/icons-material/ArrowBack";
import AppLayout from "../../../components/layout/AppLayout";
import { useEffect, useState } from "react";
import { api } from "../../../services/api";
import { useNavigate, useParams } from "react-router-dom";

interface Aula {
  id: number;
  turmaDisciplinaId: number;
  titulo: string;
  descricao: string;
  urlVideo?: string;
  materialApoio?: string;
  criadoEm: string;
}

interface Atividade {
  atividadeId: number;
  titulo: string;
  disciplinaId: number;
  nomeDisciplina: string;
  dataEntrega: string;
  jaEntregue: boolean;
  nota: number | null;
}

interface Disciplina {
  id: number;
  nome: string;
  descricao: string;
  cargaHoraria: number;
  cursoId: number;
  cursoNome: string;
  ativo: boolean;
}

export default function AlunoDisciplinaDetalhe() {
  const { disciplinaId } = useParams();
  const navigate = useNavigate();

  const [loading, setLoading] = useState(true);
  const [tab, setTab] = useState(0);
  const [aulas, setAulas] = useState<Aula[]>([]);
  const [atividades, setAtividades] = useState<Atividade[]>([]);
  const [disciplina, setDisciplina] = useState<Disciplina | null>(null);

  useEffect(() => {
    async function carregarDados() {
      try {
        if (!disciplinaId) return;

        // 🔹 buscar disciplina
        const disciplinaRes = await api.get<Disciplina>(
          `/disciplina/${disciplinaId}`
        );
        setDisciplina(disciplinaRes.data);

        // 🔹 buscar aulas
        const aulasRes = await api.get<Aula[]>("/aulas/minhas");

        // 🔹 buscar atividades
        const atividadesRes = await api.get<Atividade[]>("/atividade/minhas");

        // 🔹 resolver disciplina real da aula
        const aulasResolvidas = await Promise.all(
          aulasRes.data.map(async (aula) => {
            const tdRes = await api.get(
              `/turmadisciplina/${aula.turmaDisciplinaId}`
            );

            return {
              ...aula,
              disciplinaRealId: tdRes.data.disciplinaId
            };
          })
        );

        const aulasFiltradas = aulasResolvidas.filter(
          (a) => a.disciplinaRealId.toString() === disciplinaId
        );

        const atividadesFiltradas = atividadesRes.data.filter(
          (a) => a.disciplinaId.toString() === disciplinaId
        );

        setAulas(aulasFiltradas);
        setAtividades(atividadesFiltradas);

      } catch (error) {
        console.error("Erro ao carregar dados da disciplina:", error);
      } finally {
        setLoading(false);
      }
    }

    carregarDados();
  }, [disciplinaId]);

  // cálculo média
  const notasValidas = atividades
    .filter((a) => a.nota !== null)
    .map((a) => a.nota as number);

  const media =
    notasValidas.length > 0
      ? (
          notasValidas.reduce((acc, n) => acc + n, 0) /
          notasValidas.length
        ).toFixed(2)
      : null;

  return (
    <AppLayout>

      {/* HEADER */}
      <Card sx={{ mb: 4 }}>
        <CardContent>

          <Box
            display="flex"
            justifyContent="space-between"
            alignItems="center"
            flexWrap="wrap"
            gap={2}
          >

            <Box>
              <Typography variant="h4" fontWeight={700}>
                {disciplina?.nome || "Disciplina"}
              </Typography>

              {disciplina?.cursoNome && (
                <Typography variant="body2" color="text.secondary">
                  {disciplina.cursoNome}
                </Typography>
              )}
            </Box>

            <Button
              variant="outlined"
              startIcon={<ArrowBackIcon />}
              onClick={() => navigate("/aluno/academico")}
            >
              Voltar
            </Button>

          </Box>

        </CardContent>
      </Card>


      {/* MÉDIA */}
      {media && (
        <Card sx={{ mb: 4 }}>
          <CardContent>

            <Typography variant="h6" fontWeight={700}>
              Média Atual: {media}
            </Typography>

            <Typography variant="body2" color="text.secondary">
              Baseado nas atividades já corrigidas.
            </Typography>

          </CardContent>
        </Card>
      )}


      {/* TABS */}
      <Tabs
        value={tab}
        onChange={(_, newValue) => setTab(newValue)}
        sx={{ mb: 2 }}
      >
        <Tab label="Aulas" />
        <Tab label="Atividades" />
        <Tab label="Notas" />
      </Tabs>


      {/* CONTEÚDO */}
      <Card>
        <CardContent>

          {loading && (
            <Box display="flex" justifyContent="center" py={4}>
              <CircularProgress />
            </Box>
          )}

          {/* AULAS */}
          {!loading && tab === 0 && (
            <>
              {aulas.length === 0 && (
                <Typography color="text.secondary">
                  Nenhuma aula disponível.
                </Typography>
              )}

              {aulas.map((aula, index) => (
                <Box key={aula.id}>

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

                    <Box>
                      <Typography fontWeight={600}>
                        {aula.titulo}
                      </Typography>

                      <Typography
                        variant="body2"
                        color="text.secondary"
                      >
                        {new Date(aula.criadoEm).toLocaleDateString()}
                      </Typography>
                    </Box>

                    <Button
                      size="small"
                      variant="outlined"
                      onClick={() =>
                        navigate(`/aluno/aula/${aula.id}`)
                      }
                    >
                      Ver Aula
                    </Button>

                  </Box>

                  {index !== aulas.length - 1 && <Divider />}

                </Box>
              ))}
            </>
          )}

          {/* ATIVIDADES */}
          {!loading && tab === 1 && (
            <>
              {atividades.length === 0 && (
                <Typography color="text.secondary">
                  Nenhuma atividade disponível.
                </Typography>
              )}

              {atividades.map((atividade, index) => (
                <Box key={atividade.atividadeId}>

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

                    <Box>
                      <Typography fontWeight={600}>
                        {atividade.titulo}
                      </Typography>

                      <Typography
                        variant="body2"
                        color="text.secondary"
                      >
                        Entrega:{" "}
                        {new Date(
                          atividade.dataEntrega
                        ).toLocaleDateString()}
                      </Typography>
                    </Box>

                    <Button
                      size="small"
                      variant="outlined"
                      onClick={() =>
                        navigate(
                          `/aluno/atividade/${atividade.atividadeId}`
                        )
                      }
                    >
                      Ver Atividade
                    </Button>

                  </Box>

                  {index !== atividades.length - 1 && <Divider />}

                </Box>
              ))}
            </>
          )}

          {/* NOTAS */}
          {!loading && tab === 2 && (
            <>
              {atividades
                .filter((a) => a.nota !== null)
                .map((atividade, index) => (
                  <Box key={atividade.atividadeId}>

                    <Box
                      display="flex"
                      justifyContent="space-between"
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

                      <Typography fontWeight={600}>
                        {atividade.titulo}
                      </Typography>

                      <Typography
                        fontWeight={700}
                        color="primary"
                      >
                        {atividade.nota}
                      </Typography>

                    </Box>

                    {index !==
                      atividades.filter((a) => a.nota !== null).length - 1 && (
                      <Divider />
                    )}

                  </Box>
                ))}

              {atividades.filter((a) => a.nota !== null).length === 0 && (
                <Typography color="text.secondary">
                  Nenhuma atividade corrigida ainda.
                </Typography>
              )}
            </>
          )}

        </CardContent>
      </Card>

    </AppLayout>
  );
}