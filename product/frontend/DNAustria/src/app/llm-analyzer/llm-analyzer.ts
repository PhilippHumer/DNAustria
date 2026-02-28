import { Component } from '@angular/core';

@Component({
  selector: 'app-llm-analyzer',
  imports: [],
  templateUrl: './llm-analyzer.html',
  styleUrl: './llm-analyzer.css',
})
export class LlmAnalyzer {
  results: string | null = null;
  loading: boolean = false;

  analyze(text: string) {
    if (!text || !text.trim()) {
      this.results = 'Please enter some text to analyze.';
      return;
    }

    this.loading = true;
    this.results = null;

    fetch('/api/events/llm', {
      method: 'POST',
      headers: { 'Content-Type': 'text/plain' },
      body: text,
    })
      .then(async (res) => {
        const contentType = res.headers.get('content-type') || '';
        let body: any;
        if (contentType.includes('application/json')) {
          body = await res.json();
          this.results = JSON.stringify(body, null, 2);
        } else {
          body = await res.text();
          this.results = body;
        }
      })
      .catch((err) => {
        console.error('LLM analyze error', err);
        this.results = 'Request failed: ' + (err && err.message ? err.message : String(err));
      })
      .finally(() => {
        this.loading = false;
      });
  }
}
