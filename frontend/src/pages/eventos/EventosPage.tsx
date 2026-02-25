import { useEffect, useMemo, useState } from "react";
import {
  Typography,
  Box,
  Button,
  CircularProgress,
  Tabs,
  Tab,
  TextField,
  Paper,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Chip
} from "@mui/material";
import ArrowBackIcon from "@mui/icons-material/ArrowBack";
import { useNavigate } from "react-router-dom";
import AppLayout from "../../components/layout/AppLayout";
import { useAuth } from "../../contexts/AuthContext";
import { api } from "../../services/api";

import FullCalendar from "@fullcalendar/react";
import dayGridPlugin from "@fullcalendar/daygrid";
import timeGridPlugin from "@fullcalendar/timegrid";
import interactionPlugin from "@fullcalendar/interaction";
import ptBrLocale from "@fullcalendar/core/locales/pt-br";

import EventoModal from "./EventoModal";
import EventoViewEditModal from "./EventoViewEditModal";

interface Evento {
  id: number;
  titulo: string;
  descricao?: string;
  inicio: string;
  fim?: string;
  tipo: number;
  turmaNome?: string;
  disciplinaNome?: string;
}

const TipoUsuario = {
  SuperAdmin: 0,
  Admin: 1,
  Professor: 2,
  Aluno: 3,
} as const;

const TipoEventoLabel: Record<number, string> = {
  1: "Geral",
  2: "Prova",
  3: "Atividade",
  4: "Aula Extra",
  5: "Reunião"
};

export default function EventosPage() {
  const { user } = useAuth();
  const navigate = useNavigate();

  const [eventos, setEventos] = useState<Evento[]>([]);
  const [loading, setLoading] = useState(true);
  const [tab, setTab] = useState(0);
  const [busca, setBusca] = useState("");
  const [createModalOpen, setCreateModalOpen] = useState(false);
  const [viewModalOpen, setViewModalOpen] = useState(false);
  const [selectedEventoId, setSelectedEventoId] = useState<number | null>(null);

  const isSuperAdmin = user?.tipo === TipoUsuario.SuperAdmin;
  const isAdmin = user?.tipo === TipoUsuario.Admin;
  const isProfessor = user?.tipo === TipoUsuario.Professor;
  const isAluno = user?.tipo === TipoUsuario.Aluno;

  const podeCriar = isSuperAdmin || isAdmin || isProfessor;

  async function carregarEventos() {
    try {
      const endpoint = isAluno ? "/evento/meus" : "/evento";
      const response = await api.get(endpoint);
      setEventos(response.data);
    } catch (error) {
      console.error("Erro ao carregar eventos:", error);
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    carregarEventos();
  }, [isAluno]);

  const eventosFiltrados = useMemo(() => {
    return eventos.filter((e) =>
      e.titulo.toLowerCase().includes(busca.toLowerCase())
    );
  }, [eventos, busca]);

  const eventosFormatados = eventos.map((e) => ({
    id: e.id.toString(),
    title: `${e.titulo}${e.disciplinaNome ? ` - ${e.disciplinaNome}` : ""}`,
    start: e.inicio,
    end: e.fim,
  }));

  return (
    <AppLayout>
      {/* HEADER */}
      <Box
        display="flex"
        justifyContent="space-between"
        alignItems="center"
        mb={4}
      >
        <Typography variant="h4" sx={{ fontWeight: 600 }}>
          Gestão de Eventos
        </Typography>

        <Button
          variant="outlined"
          startIcon={<ArrowBackIcon />}
          onClick={() => navigate(-1)}
        >
          Voltar
        </Button>
      </Box>

      {/* TABS */}
      <Tabs
        value={tab}
        onChange={(_, newValue) => setTab(newValue)}
        sx={{ mb: 3 }}
      >
        <Tab label="Calendário" />
        <Tab label="Lista" />
      </Tabs>

      {loading ? (
        <Box display="flex" justifyContent="center" mt={6}>
          <CircularProgress />
        </Box>
      ) : (
        <>
          {/* CALENDÁRIO */}
          {tab === 0 && (
            <Paper sx={{ p: 2 }}>
              <FullCalendar
                plugins={[dayGridPlugin, timeGridPlugin, interactionPlugin]}
                locale={ptBrLocale}
                initialView="dayGridMonth"
                height="auto"
                headerToolbar={{
                  left: "prev,next today",
                  center: "title",
                  right:
                    (podeCriar ? "customNovoEvento " : "") +
                    "dayGridMonth,timeGridWeek,timeGridDay",
                }}
                customButtons={{
                  customNovoEvento: {
                    text: "Novo Evento",
                    click: () => setCreateModalOpen(true),
                  },
                }}
                buttonText={{
                  today: "Hoje",
                  month: "Mês",
                  week: "Semana",
                  day: "Dia",
                }}
                events={eventosFormatados}
                eventClick={(info) => {
                  setSelectedEventoId(Number(info.event.id));
                  setViewModalOpen(true);
                }}
              />
            </Paper>
          )}

          {/* LISTA */}
          {tab === 1 && (
            <>
              <Box mb={3}>
                <TextField
                  label="Pesquisar evento"
                  variant="outlined"
                  fullWidth
                  value={busca}
                  onChange={(e) => setBusca(e.target.value)}
                />
              </Box>

              <TableContainer component={Paper}>
                <Table>
                  <TableHead>
                    <TableRow>
                      <TableCell>Título</TableCell>
                      <TableCell>Tipo</TableCell>
                      <TableCell>Turma</TableCell>
                      <TableCell>Data</TableCell>
                    </TableRow>
                  </TableHead>
                  <TableBody>
                    {eventosFiltrados.map((evento) => (
                      <TableRow
                        key={evento.id}
                        hover
                        sx={{ cursor: "pointer" }}
                        onClick={() => {
                          setSelectedEventoId(evento.id);
                          setViewModalOpen(true);
                        }}
                      >
                        <TableCell>{evento.titulo}</TableCell>
                        <TableCell>
                          <Chip
                            label={TipoEventoLabel[evento.tipo] || "Outro"}
                            size="small"
                          />
                        </TableCell>
                        <TableCell>{evento.turmaNome}</TableCell>
                        <TableCell>
                          {new Date(evento.inicio).toLocaleDateString("pt-BR")}
                        </TableCell>
                      </TableRow>
                    ))}
                  </TableBody>
                </Table>
              </TableContainer>
            </>
          )}
        </>
      )}

      {/* MODAL CRIAR */}
      <EventoModal
        open={createModalOpen}
        onClose={() => setCreateModalOpen(false)}
        onSuccess={() => {
          setCreateModalOpen(false);
          setLoading(true);
          carregarEventos();
        }}
      />

      {/* MODAL VIEW/EDIT */}
      <EventoViewEditModal
        open={viewModalOpen}
        onClose={() => setViewModalOpen(false)}
        eventoId={selectedEventoId}
        podeEditar={podeCriar}
        onUpdated={() => {
          setLoading(true);
          carregarEventos();
        }}
      />
    </AppLayout>
  );
}