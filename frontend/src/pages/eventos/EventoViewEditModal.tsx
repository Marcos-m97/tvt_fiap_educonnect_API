import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  TextField,
  MenuItem,
  Box,
  CircularProgress
} from "@mui/material";
import { useEffect, useState } from "react";
import { api } from "../../services/api";

interface Evento {
  id: number;
  titulo: string;
  descricao?: string;
  inicio: string;
  fim?: string;
  tipo: number;
  turmaId?: number | null;
  turmaDisciplinaId?: number | null;
}

interface Turma {
  id: number;
  nome: string;
}

interface TurmaDisciplina {
  id: number;
  disciplinaNome: string;
  turmaNome: string;
}

interface Props {
  open: boolean;
  onClose: () => void;
  eventoId: number | null;
  podeEditar: boolean;
  onUpdated: () => void;
}

const TipoEventoOptions = [
  { value: 1, label: "Geral" },
  { value: 2, label: "Prova" },
  { value: 3, label: "Atividade" },
  { value: 4, label: "Aula Extra" },
  { value: 5, label: "Reunião" }
];

export default function EventoViewEditModal({
  open,
  onClose,
  eventoId,
  podeEditar,
  onUpdated
}: Props) {
  const [evento, setEvento] = useState<Evento | null>(null);
  const [modoEdicao, setModoEdicao] = useState(false);
  const [loading, setLoading] = useState(false);

  const [turmas, setTurmas] = useState<Turma[]>([]);
  const [turmaDisciplinas, setTurmaDisciplinas] = useState<TurmaDisciplina[]>([]);

  const [turmaId, setTurmaId] = useState<number | null>(null);
  const [turmaDisciplinaId, setTurmaDisciplinaId] = useState<number | null>(null);

  useEffect(() => {
    if (!eventoId || !open) return;

    async function carregar() {
      setLoading(true);

      const [eventoRes, turmasRes, tdRes] = await Promise.all([
        api.get(`/evento/${eventoId}`),
        api.get("/turma"),
        api.get("/turmadisciplina")
      ]);

      const data = eventoRes.data;

      setEvento(data);
      setTurmas(turmasRes.data);
      setTurmaDisciplinas(tdRes.data);

      setTurmaId(data.turmaId ?? null);
      setTurmaDisciplinaId(data.turmaDisciplinaId ?? null);

      setLoading(false);
    }

    carregar();
  }, [eventoId, open]);

  async function handleSave() {
    if (!evento) return;

    try {
      setLoading(true);

      await api.put(`/evento/${evento.id}`, {
        titulo: evento.titulo,
        descricao: evento.descricao,
        inicio: evento.inicio,
        fim: evento.fim,
        tipo: evento.tipo,
        turmaId,
        turmaDisciplinaId
      });

      setModoEdicao(false);
      onUpdated();
      onClose();
    } catch (error) {
      console.error("Erro ao atualizar evento:", error);
    } finally {
      setLoading(false);
    }
  }

  if (!evento) return null;

  return (
    <Dialog open={open} onClose={onClose} fullWidth maxWidth="sm">
      <DialogTitle>
        {modoEdicao ? "Editar Evento" : "Detalhes do Evento"}
      </DialogTitle>

      <DialogContent>
        {loading ? (
          <Box display="flex" justifyContent="center" mt={3}>
            <CircularProgress />
          </Box>
        ) : (
          <Box display="flex" flexDirection="column" gap={2} mt={1}>
            <TextField
              label="Título"
              value={evento.titulo}
              disabled={!modoEdicao}
              onChange={(e) =>
                setEvento({ ...evento, titulo: e.target.value })
              }
              fullWidth
            />

            <TextField
              label="Descrição"
              value={evento.descricao}
              disabled={!modoEdicao}
              multiline
              rows={3}
              onChange={(e) =>
                setEvento({ ...evento, descricao: e.target.value })
              }
              fullWidth
            />

            <TextField
              type="datetime-local"
              label="Início"
              InputLabelProps={{ shrink: true }}
              value={evento.inicio}
              disabled={!modoEdicao}
              onChange={(e) =>
                setEvento({ ...evento, inicio: e.target.value })
              }
              fullWidth
            />

            <TextField
              type="datetime-local"
              label="Fim"
              InputLabelProps={{ shrink: true }}
              value={evento.fim}
              disabled={!modoEdicao}
              onChange={(e) =>
                setEvento({ ...evento, fim: e.target.value })
              }
              fullWidth
            />

            <TextField
              select
              label="Tipo"
              value={evento.tipo}
              disabled={!modoEdicao}
              onChange={(e) =>
                setEvento({ ...evento, tipo: Number(e.target.value) })
              }
              fullWidth
            >
              {TipoEventoOptions.map((opt) => (
                <MenuItem key={opt.value} value={opt.value}>
                  {opt.label}
                </MenuItem>
              ))}
            </TextField>

            {/* TURMA */}
            <TextField
              select
              label="Turma (opcional)"
              value={turmaId ?? ""}
              disabled={!modoEdicao}
              onChange={(e) => {
                const value = e.target.value;
                setTurmaId(value ? Number(value) : null);
                setTurmaDisciplinaId(null);
              }}
              fullWidth
            >
              <MenuItem value="">Nenhuma</MenuItem>
              {turmas.map((t) => (
                <MenuItem key={t.id} value={t.id}>
                  {t.nome}
                </MenuItem>
              ))}
            </TextField>

            {/* TURMA-DISCIPLINA */}
            <TextField
              select
              label="Turma-Disciplina (opcional)"
              value={turmaDisciplinaId ?? ""}
              disabled={!modoEdicao}
              onChange={(e) => {
                const value = e.target.value;
                setTurmaDisciplinaId(value ? Number(value) : null);
              }}
              fullWidth
            >
              <MenuItem value="">Nenhuma</MenuItem>
              {turmaDisciplinas.map((td) => (
                <MenuItem key={td.id} value={td.id}>
                  {td.turmaNome} - {td.disciplinaNome}
                </MenuItem>
              ))}
            </TextField>
          </Box>
        )}
      </DialogContent>

      <DialogActions>
        {!modoEdicao && podeEditar && (
          <Button onClick={() => setModoEdicao(true)}>
            Editar
          </Button>
        )}

        {modoEdicao && (
          <Button
            variant="contained"
            onClick={handleSave}
            disabled={loading}
          >
            Salvar
          </Button>
        )}

        <Button onClick={onClose}>Fechar</Button>
      </DialogActions>
    </Dialog>
  );
}