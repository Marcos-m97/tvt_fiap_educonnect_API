import {
  Box,
  Typography,
  Button,
  Container,
  Card,
  CardContent,
  Stack
} from "@mui/material";
import ArrowBackIcon from "@mui/icons-material/ArrowBack";
import { useNavigate, useParams } from "react-router-dom";
import { useAuth } from "../../contexts/AuthContext";

type Curso = {
  nome: string;
  descricaoMarketing: string;
  descricaoOficial: string;
  cargaHoraria: string;
  duracao: string;
  disciplinas: string[];
};

export default function CursoPage() {
  const navigate = useNavigate();
  const { id } = useParams();
  const { user } = useAuth();

  const cursos: Record<string, Curso> = {
    "1": {
      nome: "Análise e Desenvolvimento de Sistemas",
      descricaoMarketing:
        "Prepare-se para o mercado de tecnologia com uma formação prática e orientada a projetos reais. Desenvolva aplicações modernas, APIs, sistemas web e soluções inovadoras.",
      descricaoOficial:
        "Curso tecnólogo com duração média de 2 anos e carga horária de 2.400 horas. Formação focada em desenvolvimento de software, banco de dados, arquitetura de sistemas e boas práticas de engenharia.",
      cargaHoraria: "2.400 horas",
      duracao: "2 anos",
      disciplinas: [
        "Lógica de Programação",
        "Banco de Dados",
        "Desenvolvimento Web",
        "Engenharia de Requisitos"
      ]
    },
    "2": {
      nome: "Engenharia de Software",
      descricaoMarketing:
        "Projete, desenvolva e gerencie sistemas complexos com qualidade e escalabilidade. Torne-se referência em arquitetura e processos de software.",
      descricaoOficial:
        "Curso bacharelado com duração média de 4 anos e carga horária de 3.200 horas. Ênfase em modelagem, qualidade de software, gestão de projetos e arquitetura corporativa.",
      cargaHoraria: "3.200 horas",
      duracao: "4 anos",
      disciplinas: [
        "Arquitetura de Software",
        "Qualidade de Software",
        "Gestão de Projetos",
        "DevOps"
      ]
    },
    "3": {
      nome: "Sistemas de Informação",
      descricaoMarketing:
        "Integre tecnologia e negócios para liderar a transformação digital nas organizações. Aprenda a desenvolver soluções estratégicas e orientadas a dados.",
      descricaoOficial:
        "Curso bacharelado com duração média de 4 anos e carga horária de 3.000 horas. Formação voltada à gestão de TI, desenvolvimento de sistemas e inteligência organizacional.",
      cargaHoraria: "3.000 horas",
      duracao: "4 anos",
      disciplinas: [
        "Gestão de TI",
        "Banco de Dados Avançado",
        "Business Intelligence",
        "Análise de Sistemas"
      ]
    },
    "4": {
      nome: "Ciência da Computação",
      descricaoMarketing:
        "Construa bases sólidas em algoritmos, inteligência artificial e ciência de dados. Desenvolva soluções inovadoras para problemas complexos.",
      descricaoOficial:
        "Curso bacharelado com duração média de 4 anos e carga horária de 3.200 horas. Ênfase em estruturas de dados, IA, computação científica e pesquisa aplicada.",
      cargaHoraria: "3.200 horas",
      duracao: "4 anos",
      disciplinas: [
        "Algoritmos Avançados",
        "Inteligência Artificial",
        "Estruturas de Dados",
        "Ciência de Dados"
      ]
    }
  };

  const curso = cursos[id ?? "1"];

  function handleMatricula() {
    if (user) {
      navigate(`/aluno/matricula?cursoId=${id}`);
    } else {
      navigate("/register");
    }
  }

  return (
    <Box py={10}>
      <Container maxWidth="lg">

        {/* Botão Voltar */}
        <Button
          startIcon={<ArrowBackIcon />}
          onClick={() => navigate("/")}
          sx={{ mb: 4 }}
        >
          Voltar
        </Button>

        {/* Título */}
        <Box textAlign="center" mb={6}>
          <Typography variant="h3" fontWeight={800} mb={2}>
            {curso.nome}
          </Typography>

          <Typography variant="h6" color="text.secondary">
            {curso.descricaoMarketing}
          </Typography>
        </Box>

        {/* Informações Oficiais */}
        <Card sx={{ mb: 6 }}>
          <CardContent>
            <Typography variant="h6" fontWeight={700} mb={2}>
              Informações do Curso
            </Typography>

            <Typography variant="body2" mb={2}>
              {curso.descricaoOficial}
            </Typography>

            <Stack direction="row" spacing={4} mt={2}>
              <Typography>
                <strong>Carga Horária:</strong> {curso.cargaHoraria}
              </Typography>
              <Typography>
                <strong>Duração:</strong> {curso.duracao}
              </Typography>
            </Stack>
          </CardContent>
        </Card>

        {/* Disciplinas */}
        <Card sx={{ mb: 6 }}>
          <CardContent>
            <Typography variant="h6" fontWeight={700} mb={3}>
              Principais Disciplinas
            </Typography>

            <Stack spacing={1}>
              {curso.disciplinas.map((disciplina, index) => (
                <Typography key={index}>
                  • {disciplina}
                </Typography>
              ))}
            </Stack>
          </CardContent>
        </Card>

        {/* Botão Matricule-se */}
        <Box textAlign="center">
          <Button
            variant="contained"
            size="large"
            sx={{ px: 6, py: 1.5 }}
            onClick={handleMatricula}
          >
            Matricule-se
          </Button>
        </Box>

      </Container>
    </Box>
  );
}