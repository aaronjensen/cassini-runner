require 'rake'
require 'fileutils'
require 'pathname'

site_dir = "TestSite"
port = "888"
test_url = "diagnostics/ok"
test_response = "OK"

namespace :site do
  task :start => :stop do
    cp 'bin/Cassini/Cassini.dll', "#{site_dir}/bin/"
    sh "cmd", "/c", "start", "bin/Cassini/CassiniRunner.exe", File.expand_path(site_dir).gsub(/\//, '\\'), port, "/"
  end

  task :stop do
    sh "bin/Cassini/CassiniRunner.exe", "stop", port
  end

  task :test => :start do
    result = `bin/curl.exe http://localhost:#{port}/#{test_url}`
    tries = 0

    while $? == 1792
      ++tries
      raise "Couldn't connect in 10 tries" if tries > 10
      result = `bin/curl.exe http://localhost:#{port}/#{test_url}`
    end

    Rake::Task["site:stop"].execute nil

    result.strip!
    test_response.strip!
    if result != test_response
      raise "Site's broken\ngot:      '" + result + "' \nexpected: '#{test_response}'"
    end
  end
end
